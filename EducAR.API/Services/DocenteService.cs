using EducAR.API.Data;
using EducAR.API.DTOs.Docentes;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Helpers;

namespace EducAR.API.Services;

public class DocenteService : IDocenteService
{
    private readonly IDocenteRepository _docenteRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly AppDbContext _context;

    private const int ID_ROL_DOCENTE = 2;

    public DocenteService(
        IDocenteRepository docenteRepository,
        IUsuarioRepository usuarioRepository,
        AppDbContext context)
    {
        _docenteRepository = docenteRepository;
        _usuarioRepository = usuarioRepository;
        _context = context;
    }

    public async Task<List<DocenteResponseDto>> ObtenerTodos(int idEscuela)
    {
        var docentes = await _docenteRepository.ObtenerTodos(idEscuela);
        return docentes.Select(MapearAResponseDto).ToList();
    }

    public async Task<DocenteResponseDto?> ObtenerPorId(int idDocente, int idEscuela)
    {
        var docente = await _docenteRepository.ObtenerPorId(idDocente, idEscuela);
        return docente is null ? null : MapearAResponseDto(docente);
    }

    public async Task<(bool exito, string mensaje, DocenteResponseDto? docente)> Crear(DocenteCreateDto dto, int idEscuela)
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
                usuarioInactivo.Nombre = dto.Nombre;
                usuarioInactivo.Apellido = dto.Apellido;
                usuarioInactivo.Email = dto.Email;
                usuarioInactivo.Dni = dto.Dni;
                usuarioInactivo.HashContrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);
                usuarioInactivo.Activo = true;
                usuarioInactivo.FechaAct = DateTime.Now;
                await _usuarioRepository.Actualizar(usuarioInactivo);
                usuario = usuarioInactivo;
            }
            else
            {
                usuario = new Usuario
                {
                    IdRol = ID_ROL_DOCENTE,
                    IdEscuela = idEscuela,
                    Dni = dto.Dni,
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Email = dto.Email,
                    NombreUsuario = dto.NombreUsuario,
                    HashContrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
                    Activo = true
                };
                await _usuarioRepository.Crear(usuario);
            }

            var docenteInactivo = await _docenteRepository.ObtenerPorUsuarioInactivo(usuario.IdUsuario);
            if (docenteInactivo is not null)
            {
                await _docenteRepository.Reactivar(docenteInactivo);
            }
            else
            {
                var docente = new Docente
                {
                    IdUsuario = usuario.IdUsuario,
                    Activo = true
                };
                await _docenteRepository.Crear(docente);
            }

            await transaction.CommitAsync();

            var completo = await _docenteRepository.ObtenerTodos(idEscuela);
            var docenteCreado = completo.FirstOrDefault(d => d.IdUsuario == usuario.IdUsuario);
            return (true, "Docente creado correctamente.", MapearAResponseDto(docenteCreado!));
        }
        catch
        {
            await transaction.RollbackAsync();
            return (false, "Ocurrió un error al crear el docente.", null);
        }
    }

    // public async Task<(bool exito, string mensaje)> Actualizar(int idDocente, int idEscuela, DocenteUpdateDto dto)
    // {
    //     var docente = await _docenteRepository.ObtenerPorId(idDocente, idEscuela);
    //     if (docente is null)
    //         return (false, "Docente no encontrado.");

    //     docente.Usuario.Nombre   = dto.Nombre;
    //     docente.Usuario.Apellido = dto.Apellido;
    //     docente.Usuario.Email    = dto.Email;
    //     docente.Usuario.Activo   = dto.Activo;
    //     docente.Activo           = dto.Activo;

    //     await _docenteRepository.Actualizar(docente);
    //     return (true, "Docente actualizado correctamente.");
    // }
    public async Task<(bool exito, string mensaje)> Actualizar(int idDocente, int idEscuela, DocenteUpdateDto dto)
    {
        var docente = await _docenteRepository.ObtenerPorId(idDocente, idEscuela);
        if (docente is null)
            return (false, "Docente no encontrado.");


        if (await _usuarioRepository.ExisteDni(dto.Dni, idEscuela, docente.IdUsuario))
            return (false, $"El DNI '{dto.Dni}' ya está siendo usado por otro usuario en esta escuela.");

        // Validar email duplicado ANTES de intentar guardar
        if (!string.Equals(docente.Usuario.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _usuarioRepository.ExisteEmail(dto.Email, idEscuela, docente.IdUsuario))
                return (false, $"El email '{dto.Email}' ya está siendo usado por otro usuario en esta escuela.");
        }

        docente.Usuario.Nombre = dto.Nombre;
        docente.Usuario.Apellido = dto.Apellido;
        docente.Usuario.Email = dto.Email;
        docente.Usuario.Activo = dto.Activo;
        docente.Usuario.Dni = dto.Dni;
        docente.Activo = dto.Activo;

        await _docenteRepository.Actualizar(docente);
        return (true, "Docente actualizado correctamente.");
    }
    public async Task<bool> Eliminar(int idDocente, int idEscuela)
    {
        return await _docenteRepository.Eliminar(idDocente, idEscuela);
    }

    public async Task<ResultadoPaginadoDto<DocenteResponseDto>> ObtenerTodosPaginado(int idEscuela, FiltroPersonaDto filtro)
    {
        var query = await _docenteRepository.ObtenerQueryable(idEscuela, filtro.Nombre, filtro.Apellido, filtro.Dni);
        var queryDto = query.Select(d => new DocenteResponseDto
        {
            IdDocente = d.IdDocente,
            IdUsuario = d.IdUsuario,
            Dni = d.Usuario.Dni,
            Nombre = d.Usuario.Nombre,
            Apellido = d.Usuario.Apellido,
            Email = d.Usuario.Email,
            NombreUsuario = d.Usuario.NombreUsuario,
            Activo = d.Activo
        });

        return await PaginacionHelper.PaginarAsync(queryDto, filtro.Pagina, filtro.Cantidad);
    }

    private static DocenteResponseDto MapearAResponseDto(Docente d) => new()
    {
        IdDocente = d.IdDocente,
        IdUsuario = d.IdUsuario,
        Dni = d.Usuario.Dni,
        Nombre = d.Usuario.Nombre,
        Apellido = d.Usuario.Apellido,
        Email = d.Usuario.Email,
        NombreUsuario = d.Usuario.NombreUsuario,
        Activo = d.Activo
    };
}
