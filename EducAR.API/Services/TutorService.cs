using EducAR.API.Data;
using EducAR.API.DTOs.Tutores;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Helpers;

namespace EducAR.API.Services;

public class TutorService : ITutorService
{
    private readonly ITutorRepository _tutorRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly AppDbContext _context;

    private const int ID_ROL_TUTOR = 3;

    public TutorService(
        ITutorRepository tutorRepository,
        IUsuarioRepository usuarioRepository,
        AppDbContext context)
    {
        _tutorRepository   = tutorRepository;
        _usuarioRepository = usuarioRepository;
        _context           = context;
    }

    public async Task<List<TutorResponseDto>> ObtenerTodos(int idEscuela)
    {
        var tutores = await _tutorRepository.ObtenerTodos(idEscuela);
        return tutores.Select(MapearAResponseDto).ToList();
    }

    public async Task<TutorResponseDto?> ObtenerPorId(int idTutor, int idEscuela)
    {
        var tutor = await _tutorRepository.ObtenerPorId(idTutor, idEscuela);
        return tutor is null ? null : MapearAResponseDto(tutor);
    }

    public async Task<TutorDetalleDto?> ObtenerDetalle(int idTutor, int idEscuela)
    {
        var tutor = await _tutorRepository.ObtenerDetalle(idTutor, idEscuela);
        if (tutor is null) return null;
        return new TutorDetalleDto
        {
            IdTutor = tutor.IdTutor, Dni = tutor.Usuario.Dni, Nombre = tutor.Usuario.Nombre,
            Apellido = tutor.Usuario.Apellido, Email = tutor.Usuario.Email,
            Activo = tutor.Usuario.Activo,
            AlumnosAsignados = tutor.AlumnoTutores.Where(at => at.Activo && at.Alumno.Activo)
                .Select(at =>
                {
                    var matricula = at.Alumno.Matriculas.FirstOrDefault(m =>
                        m.Estado == EstadoMatricula.Activa && m.CicloLectivo.Activo);
                    return new AlumnoTutorDetalleDto
                    {
                        IdAlumno = at.Alumno.IdAlumno, Dni = at.Alumno.Dni,
                        NombreCompleto = $"{at.Alumno.Nombre} {at.Alumno.Apellido}",
                        CursoActual = matricula is null ? string.Empty : $"{matricula.Curso.Grado}° {matricula.Curso.Division}"
                    };
                }).ToList()
        };
    }

    public async Task<(bool exito, string mensaje, TutorResponseDto? tutor)> Crear(TutorCreateDto dto, int idEscuela)
    {
        if (await _usuarioRepository.ExisteNombreUsuario(dto.NombreUsuario, idEscuela))
            return (false, "El nombre de usuario ya existe en esta escuela.", null);

        if (await _usuarioRepository.ExisteDni(dto.Dni, idEscuela))
            return (false, "Ya existe un usuario con ese DNI en esta escuela.", null);

        if (await _usuarioRepository.ExisteEmail(dto.Email, idEscuela))
            return (false, "Ya existe un usuario con ese email en esta escuela.", null);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var usuarioInactivo = await _usuarioRepository
                .ObtenerUsuarioInactivoPorNombre(dto.NombreUsuario, idEscuela);

            Usuario usuario;
            if (usuarioInactivo is not null)
            {
                // Buscar tutor inactivo ANTES de reactivar el usuario
                var tutorInactivo = await _tutorRepository
                    .ObtenerPorUsuarioInactivo(usuarioInactivo.IdUsuario);

                // Reactivar usuario
                usuarioInactivo.Nombre         = dto.Nombre;
                usuarioInactivo.Apellido       = dto.Apellido;
                usuarioInactivo.Email          = dto.Email;
                usuarioInactivo.Dni            = dto.Dni;
                usuarioInactivo.HashContrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);
                usuarioInactivo.Activo         = true;
                usuarioInactivo.FechaAct       = DateTime.Now;
                await _usuarioRepository.Actualizar(usuarioInactivo);
                usuario = usuarioInactivo;

                if (tutorInactivo is not null)
                {
                    tutorInactivo.EsResponsable = dto.EsResponsable;
                    await _tutorRepository.Reactivar(tutorInactivo);
                }
                else
                {
                    var tutorNuevo = new Tutor
                    {
                        IdUsuario     = usuario.IdUsuario,
                        EsResponsable = dto.EsResponsable
                    };
                    await _tutorRepository.Crear(tutorNuevo);
                }
            }
            else
            {
                usuario = new Usuario
                {
                    IdRol          = ID_ROL_TUTOR,
                    IdEscuela      = idEscuela,
                    Dni            = dto.Dni,
                    Nombre         = dto.Nombre,
                    Apellido       = dto.Apellido,
                    Email          = dto.Email,
                    NombreUsuario  = dto.NombreUsuario,
                    HashContrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
                    Activo         = true
                };
                await _usuarioRepository.Crear(usuario);

                var tutor = new Tutor
                {
                    IdUsuario     = usuario.IdUsuario,
                    EsResponsable = dto.EsResponsable
                };
                await _tutorRepository.Crear(tutor);
            }

            await transaction.CommitAsync();

            var completo    = await _tutorRepository.ObtenerTodos(idEscuela);
            var tutorCreado = completo.FirstOrDefault(t => t.IdUsuario == usuario.IdUsuario);
            return (true, "Tutor creado correctamente.", MapearAResponseDto(tutorCreado!));
        }
        catch
        {
            await transaction.RollbackAsync();
            return (false, "Ocurrió un error al crear el tutor.", null);
        }
    }

    public async Task<(bool exito, string mensaje)> Actualizar(int idTutor, int idEscuela, TutorUpdateDto dto)
    {
        var tutor = await _tutorRepository.ObtenerPorId(idTutor, idEscuela);
        if (tutor is null)
            return (false, "Tutor no encontrado.");

        if (await _usuarioRepository.ExisteDni(dto.Dni, idEscuela, tutor.IdUsuario))
            return (false, "Ya existe un usuario con ese DNI en esta escuela.");

        if (!string.Equals(tutor.Usuario.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _usuarioRepository.ExisteEmail(dto.Email, idEscuela, tutor.IdUsuario))
                return (false, $"El email '{dto.Email}' ya está siendo usado por otro usuario en esta escuela.");
        }

        tutor.Usuario.Dni      = dto.Dni;
        tutor.Usuario.Nombre   = dto.Nombre;
        tutor.Usuario.Apellido = dto.Apellido;
        tutor.Usuario.Email    = dto.Email;
        tutor.Usuario.Activo   = dto.Activo;
        tutor.EsResponsable    = dto.EsResponsable;

        await _tutorRepository.Actualizar(tutor);
        return (true, "Tutor actualizado correctamente.");
    }

    public async Task<bool> Eliminar(int idTutor, int idEscuela)
    {
        return await _tutorRepository.Eliminar(idTutor, idEscuela);
    }

    public async Task<ResultadoPaginadoDto<TutorResponseDto>> ObtenerTodosPaginado(int idEscuela, FiltroPersonaDto filtro)
    {
        var query = await _tutorRepository.ObtenerQueryable(idEscuela, filtro.Nombre, filtro.Apellido, filtro.Dni);
        var queryDto = query.Select(t => new TutorResponseDto
        {
            IdTutor       = t.IdTutor,
            IdUsuario     = t.IdUsuario,
            Dni           = t.Usuario.Dni,
            Nombre        = t.Usuario.Nombre,
            Apellido      = t.Usuario.Apellido,
            Email         = t.Usuario.Email,
            NombreUsuario = t.Usuario.NombreUsuario,
            EsResponsable = t.EsResponsable,
            Activo        = t.Usuario.Activo
        });

        return await PaginacionHelper.PaginarAsync(queryDto, filtro.Pagina, filtro.Cantidad);
    }

    private static TutorResponseDto MapearAResponseDto(Tutor t) => new()
    {
        IdTutor       = t.IdTutor,
        IdUsuario     = t.IdUsuario,
        Dni           = t.Usuario.Dni,
        Nombre        = t.Usuario.Nombre,
        Apellido      = t.Usuario.Apellido,
        Email         = t.Usuario.Email,
        NombreUsuario = t.Usuario.NombreUsuario,
        EsResponsable = t.EsResponsable,
        Activo        = t.Usuario.Activo
    };
}
