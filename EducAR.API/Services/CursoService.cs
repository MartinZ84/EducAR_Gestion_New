using EducAR.API.DTOs.Cursos;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Helpers;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;

namespace EducAR.API.Services;

public class CursoService : ICursoService
{
    private readonly ICursoRepository _cursoRepository;
    private readonly ICicloLectivoRepository _cicloLectivoRepository;

    public CursoService(
        ICursoRepository cursoRepository,
        ICicloLectivoRepository cicloLectivoRepository)
    {
        _cursoRepository        = cursoRepository;
        _cicloLectivoRepository = cicloLectivoRepository;
    }

    public async Task<List<CursoResponseDto>> ObtenerTodos(int idEscuela)
    {
        var cursos = await _cursoRepository.ObtenerTodos(idEscuela);
        return cursos.Select(MapearAResponseDto).ToList();
    }

    public async Task<List<CursoResponseDto>> ObtenerPorCicloLectivo(int idCicloLectivo, int idEscuela)
    {
        var cursos = await _cursoRepository.ObtenerPorCicloLectivo(idCicloLectivo, idEscuela);
        return cursos.Select(MapearAResponseDto).ToList();
    }

    public async Task<CursoResponseDto?> ObtenerPorId(int idCurso, int idEscuela)
    {
        var curso = await _cursoRepository.ObtenerPorId(idCurso, idEscuela);
        return curso is null ? null : MapearAResponseDto(curso);
    }

    public async Task<(bool exito, string mensaje, CursoResponseDto? curso)> Crear(CursoCreateDto dto, int idEscuela)
    {
        var ciclo = await _cicloLectivoRepository.ObtenerPorId(dto.IdCicloLectivo, idEscuela);
        if (ciclo is null)
            return (false, "El ciclo lectivo no existe.", null);

        if (!ciclo.Activo)
            return (false, "No se puede crear un curso en un ciclo lectivo inactivo.", null);

        if (await _cursoRepository.ExisteCurso(dto.Grado, dto.Division, dto.Turno, dto.IdCicloLectivo))
            return (false, $"Ya existe el {dto.Grado}° '{dto.Division}' turno {dto.Turno} en este ciclo lectivo.", null);

        var curso = new Curso
        {
            IdEscuela      = idEscuela,
            IdCicloLectivo = dto.IdCicloLectivo,
            Grado          = dto.Grado,
            Division       = dto.Division,
            Turno          = dto.Turno,
            Activo         = true
        };

        var creado   = await _cursoRepository.Crear(curso);
        var completo = await _cursoRepository.ObtenerPorId(creado.IdCurso, idEscuela);
        return (true, "Curso creado correctamente.", MapearAResponseDto(completo!));
    }

    public async Task<(bool exito, string mensaje)> Actualizar(int idCurso, int idEscuela, CursoUpdateDto dto)
    {
        var curso = await _cursoRepository.ObtenerPorId(idCurso, idEscuela);
        if (curso is null)
            return (false, "Curso no encontrado.");

        if (await _cursoRepository.ExisteCurso(dto.Grado, dto.Division, dto.Turno, curso.IdCicloLectivo, idCurso))
            return (false, $"Ya existe el {dto.Grado}° '{dto.Division}' turno {dto.Turno} en este ciclo lectivo.");

        curso.Grado    = dto.Grado;
        curso.Division = dto.Division;
        curso.Turno    = dto.Turno;
        curso.Activo   = dto.Activo;

        await _cursoRepository.Actualizar(curso);
        return (true, "Curso actualizado correctamente.");
    }

    public async Task<(bool exito, string mensaje)> Eliminar(int idCurso, int idEscuela)
    {
        var curso = await _cursoRepository.ObtenerPorId(idCurso, idEscuela);
        if (curso is null)
            return (false, "Curso no encontrado.");

        if (await _cursoRepository.TieneAlumnosInscriptos(idCurso))
            return (false, "No se puede eliminar el curso porque tiene alumnos inscriptos activos.");

        if (await _cursoRepository.TieneAsistencias(idCurso))
            return (false, "No se puede eliminar el curso porque tiene asistencias registradas.");

        if (await _cursoRepository.TieneCalificaciones(idCurso))
            return (false, "No se puede eliminar el curso porque tiene boletines generados.");

        await _cursoRepository.Eliminar(idCurso, idEscuela);
        return (true, "Curso dado de baja correctamente.");
    }

    public async Task<ResultadoPaginadoDto<CursoResponseDto>> ObtenerTodosPaginado(int idEscuela, FiltroCursoDto filtro)
    {
        var query = await _cursoRepository.ObtenerQueryable(
            idEscuela, filtro.Grado, filtro.Division, filtro.Turno, filtro.IdCicloLectivo);

        var queryDto = query.Select(c => new CursoResponseDto
        {
            IdCurso         = c.IdCurso,
            IdEscuela       = c.IdEscuela,
            IdCicloLectivo  = c.IdCicloLectivo,
            Anio            = c.CicloLectivo.Anio,
            Grado           = c.Grado,
            Division        = c.Division,
            Turno           = c.Turno,
            Activo          = c.Activo,
            CantidadAlumnos = c.Matriculas.Count(m => m.Estado == EstadoMatricula.Activa)
        });

        return await PaginacionHelper.PaginarAsync(queryDto, filtro.Pagina, filtro.Cantidad);
    }

    private static CursoResponseDto MapearAResponseDto(Curso c) => new()
    {
        IdCurso         = c.IdCurso,
        IdEscuela       = c.IdEscuela,
        IdCicloLectivo  = c.IdCicloLectivo,
        Anio            = c.CicloLectivo.Anio,
        Grado           = c.Grado,
        Division        = c.Division,
        Turno           = c.Turno,
        Activo          = c.Activo,
        CantidadAlumnos = c.Matriculas.Count(m => m.Estado == EstadoMatricula.Activa)
    };
}
