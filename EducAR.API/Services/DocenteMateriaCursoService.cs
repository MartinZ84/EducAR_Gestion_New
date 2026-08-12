using EducAR.API.DTOs.DocenteMateriaCurso;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;
using EducAR.API.Data;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Services;

public class DocenteMateriaCursoService : IDocenteMateriaCursoService
{
    private readonly IDocenteMateriaCursoRepository _docenteMateriaCursoRepository;
    private readonly ICursoRepository _cursoRepository;
    private readonly AppDbContext _context;

    public DocenteMateriaCursoService(
        IDocenteMateriaCursoRepository docenteMateriaCursoRepository,
        ICursoRepository cursoRepository,
        AppDbContext context)
    {
        _docenteMateriaCursoRepository = docenteMateriaCursoRepository;
        _cursoRepository               = cursoRepository;
        _context                       = context;
    }

    public async Task<List<DocenteMateriaCursoResponseDto>> ObtenerPorDocente(int idDocente, int idEscuela)
    {
        var docente = await _context.Docentes
            .Include(d => d.Usuario)
            .FirstOrDefaultAsync(d => d.IdDocente == idDocente && d.Usuario.IdEscuela == idEscuela);
        if (docente is null) return new List<DocenteMateriaCursoResponseDto>();

        var asignaciones = await _docenteMateriaCursoRepository.ObtenerPorDocente(idDocente);
        return asignaciones.Select(MapearAResponseDto).ToList();
    }

    public async Task<List<DocenteMateriaCursoResponseDto>> ObtenerPorCurso(int idCurso, int idEscuela)
    {
        var curso = await _cursoRepository.ObtenerPorId(idCurso, idEscuela);
        if (curso is null) return new List<DocenteMateriaCursoResponseDto>();

        var asignaciones = await _docenteMateriaCursoRepository.ObtenerPorCurso(idCurso);
        return asignaciones.Select(MapearAResponseDto).ToList();
    }

    public async Task<List<DocenteMateriaCursoResponseDto>> ObtenerPorCursoYMateria(int idCurso, int idMateria, int idEscuela)
    {
        var curso = await _cursoRepository.ObtenerPorId(idCurso, idEscuela);
        if (curso is null) return new List<DocenteMateriaCursoResponseDto>();

        var asignaciones = await _docenteMateriaCursoRepository.ObtenerPorCursoYMateria(idCurso, idMateria);
        return asignaciones.Select(MapearAResponseDto).ToList();
    }

    public async Task<(bool exito, string mensaje, DocenteMateriaCursoResponseDto? asignacion)> Asignar(
        DocenteMateriaCursoCreateDto dto, int idEscuela)
    {
        // Validar que el curso pertenece a la escuela
        var curso = await _cursoRepository.ObtenerPorId(dto.IdCurso, idEscuela);
        if (curso is null)
            return (false, "El curso no existe.", null);

        if (!curso.Activo)
            return (false, "El curso no está activo.", null);

        // Validar que el docente pertenece a la escuela
        var docente = await _context.Docentes
            .Include(d => d.Usuario)
            .FirstOrDefaultAsync(d => d.IdDocente == dto.IdDocente &&
                                      d.Usuario.IdEscuela == idEscuela &&
                                      d.Activo);
        if (docente is null)
            return (false, "El docente no existe.", null);

        // Validar que la materia pertenece a la escuela
        var materia = await _context.Materias
            .FirstOrDefaultAsync(m => m.IdMateria == dto.IdMateria &&
                                      m.IdEscuela == idEscuela &&
                                      m.Activo);
        if (materia is null)
            return (false, "La materia no existe.", null);

        // Validar asignación activa duplicada
        if (await _docenteMateriaCursoRepository.ExisteAsignacion(dto.IdDocente, dto.IdMateria, dto.IdCurso))
            return (false, "El docente ya está asignado a esta materia y curso.", null);

        // Reactivar si existe inactiva
        var inactiva = await _docenteMateriaCursoRepository
            .ObtenerAsignacionInactiva(dto.IdDocente, dto.IdMateria, dto.IdCurso);

        if (inactiva is not null)
        {
            var reactivada = await _docenteMateriaCursoRepository.Reactivar(inactiva);
            var completaReactivada = await _docenteMateriaCursoRepository.ObtenerPorId(reactivada.IdDocenteMateriaCurso);
            return (true, "Asignación reactivada correctamente.", MapearAResponseDto(completaReactivada!));
        }

        // Crear nueva asignación
        var asignacion = new DocenteMateriaCurso
        {
            IdDocente       = dto.IdDocente,
            IdMateria       = dto.IdMateria,
            IdCurso         = dto.IdCurso,
            FechaAsignacion = DateTime.Now,
            Activo          = true
        };

        var creada = await _docenteMateriaCursoRepository.Crear(asignacion);
        var completa = await _docenteMateriaCursoRepository.ObtenerPorId(creada.IdDocenteMateriaCurso);
        return (true, "Docente asignado correctamente.", MapearAResponseDto(completa!));
    }

    public async Task<(bool exito, string mensaje)> Desasignar(int idDocenteMateriaCurso, int idEscuela)
    {
        var asignacion = await _docenteMateriaCursoRepository.ObtenerPorId(idDocenteMateriaCurso);
        if (asignacion is null)
            return (false, "Asignación no encontrada.");

        if (asignacion.Curso.IdEscuela != idEscuela)
            return (false, "No tiene permisos para modificar esta asignación.");

        var exito = await _docenteMateriaCursoRepository.Eliminar(idDocenteMateriaCurso, idEscuela);
        return exito
            ? (true, "Docente desasignado correctamente.")
            : (false, "No se pudo desasignar el docente.");
    }

    private static DocenteMateriaCursoResponseDto MapearAResponseDto(DocenteMateriaCurso dmc) => new()
    {
        IdDocenteMateriaCurso = dmc.IdDocenteMateriaCurso,
        IdDocente             = dmc.IdDocente,
        NombreDocente         = dmc.Docente.Usuario.Nombre,
        ApellidoDocente       = dmc.Docente.Usuario.Apellido,
        IdMateria             = dmc.IdMateria,
        NombreMateria         = dmc.Materia.Nombre,
        IdCurso               = dmc.IdCurso,
        Curso                 = $"{dmc.Curso.Grado}° {dmc.Curso.Division} - {dmc.Curso.CicloLectivo.Anio}",
        FechaAsignacion       = dmc.FechaAsignacion,
        Activo                = dmc.Activo
    };

    public async Task<List<DocenteMateriaCursoResponseDto>> ObtenerMisCursos(int idUsuario, int idEscuela)
    {
        // Obtener el IdDocente a partir del IdUsuario del token
        var docente = await _context.Docentes
            .FirstOrDefaultAsync(d => d.IdUsuario == idUsuario);

        if (docente is null) return new List<DocenteMateriaCursoResponseDto>();

        var asignaciones = await _docenteMateriaCursoRepository.ObtenerPorDocente(docente.IdDocente);
        return asignaciones.Select(MapearAResponseDto).ToList();
    }
}