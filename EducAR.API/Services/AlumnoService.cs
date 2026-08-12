using EducAR.API.DTOs.Alumnos;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Helpers;

namespace EducAR.API.Services;

public class AlumnoService : IAlumnoService
{
    private readonly IAlumnoRepository _alumnoRepository;

    public AlumnoService(IAlumnoRepository alumnoRepository)
    {
        _alumnoRepository = alumnoRepository;
    }

    public async Task<List<AlumnoResponseDto>> ObtenerTodos(int idEscuela)
    {
        var alumnos = await _alumnoRepository.ObtenerTodos(idEscuela);
        return alumnos.Select(MapearAResponseDto).ToList();
    }

    public async Task<AlumnoResponseDto?> ObtenerPorId(int idAlumno, int idEscuela)
    {
        var alumno = await _alumnoRepository.ObtenerPorId(idAlumno, idEscuela);
        return alumno is null ? null : MapearAResponseDto(alumno);
    }

    public async Task<(bool exito, string mensaje, AlumnoResponseDto? alumno)> Crear(AlumnoCreateDto dto, int idEscuela)
    {
        if (dto.FecNac == default || dto.FecNac > DateOnly.FromDateTime(DateTime.Today))
            return (false, "La fecha de nacimiento no es válida.", null);

        if (await _alumnoRepository.ExisteDni(dto.Dni, idEscuela))
            return (false, "Ya existe un alumno con ese DNI en esta escuela.", null);

        var alumno = new Alumno
        {
            IdEscuela = idEscuela,
            Dni       = dto.Dni,
            Nombre    = dto.Nombre,
            Apellido  = dto.Apellido,
            FecNac    = dto.FecNac,
            Activo    = true
        };

        await _alumnoRepository.Crear(alumno);
        return (true, "Alumno creado correctamente.", MapearAResponseDto(alumno));
    }

    public async Task<(bool exito, string mensaje)> Actualizar(int idAlumno, int idEscuela, AlumnoUpdateDto dto)
    {
        if (dto.FecNac == default || dto.FecNac > DateOnly.FromDateTime(DateTime.Today))
            return (false, "La fecha de nacimiento no es válida.");

        var alumno = await _alumnoRepository.ObtenerPorId(idAlumno, idEscuela);
        if (alumno is null)
            return (false, "Alumno no encontrado.");

        alumno.Nombre   = dto.Nombre;
        alumno.Apellido = dto.Apellido;
        alumno.FecNac   = dto.FecNac;
        alumno.Activo   = dto.Activo;

        await _alumnoRepository.Actualizar(alumno);
        return (true, "Alumno actualizado correctamente.");
    }

    public async Task<bool> Eliminar(int idAlumno, int idEscuela)
    {
        return await _alumnoRepository.Eliminar(idAlumno, idEscuela);
    }

    // ==========================
    // TUTORES
    // ==========================

    public async Task<(bool exito, string mensaje)> AsociarTutor(int idAlumno, int idEscuela, AsociarTutorDto dto)
    {
        var alumno = await _alumnoRepository.ObtenerPorId(idAlumno, idEscuela);
        if (alumno is null)
            return (false, "Alumno no encontrado.");

        if (!await _alumnoRepository.ExisteTutorEnEscuela(dto.IdTutor, idEscuela))
            return (false, "El tutor no existe o no pertenece a esta escuela.");

        var asociacion = await _alumnoRepository.ObtenerAsociacionTutor(idAlumno, dto.IdTutor);
        if (asociacion is not null)
        {
            if (asociacion.Activo)
                return (false, "El tutor ya está asociado a ese alumno.");

            asociacion.Activo             = true;
            asociacion.Parentesco         = dto.Parentesco;
            asociacion.EsResponsablePrinc = dto.EsResponsablePrinc;
            await _alumnoRepository.GuardarCambios();
            return (true, "Tutor reasociado al alumno correctamente.");
        }

        var nueva = new AlumnoTutor
        {
            IdAlumno           = idAlumno,
            IdTutor            = dto.IdTutor,
            Parentesco         = dto.Parentesco,
            EsResponsablePrinc = dto.EsResponsablePrinc,
            Activo             = true
        };

        await _alumnoRepository.AsociarTutor(nueva);
        return (true, "Tutor asociado al alumno correctamente.");
    }

    public async Task<(bool exito, string mensaje)> QuitarTutor(int idAlumno, int idEscuela, int idTutor)
    {
        var alumno = await _alumnoRepository.ObtenerPorId(idAlumno, idEscuela);
        if (alumno is null)
            return (false, "Alumno no encontrado.");

        var exito = await _alumnoRepository.QuitarTutor(idAlumno, idTutor);
        if (!exito)
            return (false, "El tutor no tiene una asociación activa con ese alumno.");

        return (true, "Tutor desasociado del alumno correctamente.");
    }

    public async Task<ResultadoPaginadoDto<AlumnoResponseDto>> ObtenerTodosPaginado(int idEscuela, FiltroPersonaDto filtro)
    {
        var query = await _alumnoRepository.ObtenerQueryable(idEscuela, filtro.Nombre, filtro.Apellido, filtro.Dni);
        var queryDto = query.Select(a => new AlumnoResponseDto
        {
            IdAlumno = a.IdAlumno,
            Nombre   = a.Nombre,
            Apellido = a.Apellido,
            Dni      = a.Dni,
            FecNac   = a.FecNac,
            Activo   = a.Activo
        });

        return await PaginacionHelper.PaginarAsync(queryDto, filtro.Pagina, filtro.Cantidad);
    }

    private static AlumnoResponseDto MapearAResponseDto(Alumno a) => new()
    {
        IdAlumno = a.IdAlumno,
        Dni      = a.Dni,
        Nombre   = a.Nombre,
        Apellido = a.Apellido,
        FecNac   = a.FecNac,
        Activo   = a.Activo,
        Matriculas = a.Matriculas?.Select(m => new AlumnoMatriculaDto
        {
            IdMatricula = m.IdMatricula,
            IdCurso = m.IdCurso,
            Grado = m.Curso.Grado,
            Division = m.Curso.Division,
            Turno = m.Curso.Turno,
            IdCicloLectivo = m.IdCicloLectivo,
            Anio = m.CicloLectivo.Anio,
            Estado = m.Estado.ToString(),
            FechaMatricula = m.FechaMatricula,
            FechaBaja = m.FechaBaja
        }).ToList() ?? new(),
        Tutores  = a.AlumnoTutores?.Select(at => new AlumnoTutorDto
        {
            IdAlumnoTutor      = at.IdAlumnoTutor,
            IdTutor            = at.IdTutor,
            Nombre             = at.Tutor.Usuario.Nombre,
            Apellido           = at.Tutor.Usuario.Apellido,
            Parentesco         = at.Parentesco,
            EsResponsablePrinc = at.EsResponsablePrinc,
            Activo             = at.Activo
        }).ToList() ?? new()
    };
}
