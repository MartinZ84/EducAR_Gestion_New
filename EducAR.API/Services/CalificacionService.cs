using EducAR.API.DTOs.Calificaciones;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;

namespace EducAR.API.Services;

public class CalificacionService : ICalificacionService
{
    private readonly ICalificacionRepository _calificacionRepository;
    private readonly ICursoRepository _cursoRepository;
    private readonly IMateriaRepository _materiaRepository;
    private readonly IPeriodoEvaluacionRepository _periodoRepository;

    public CalificacionService(
        ICalificacionRepository calificacionRepository,
        ICursoRepository cursoRepository,
        IMateriaRepository materiaRepository,
        IPeriodoEvaluacionRepository periodoRepository)
    {
        _calificacionRepository = calificacionRepository;
        _cursoRepository = cursoRepository;
        _materiaRepository = materiaRepository;
        _periodoRepository = periodoRepository;
    }

    public async Task<CalificacionPorCursoResponseDto?> ObtenerPorCursoMateriaYPeriodo(
        int idCurso, int idMateria, int idPeriodo, int idEscuela)
    {
        var curso = await _cursoRepository.ObtenerPorId(idCurso, idEscuela);
        if (curso is null) return null;

        var materia = await _materiaRepository.ObtenerPorId(idMateria, idEscuela);
        if (materia is null) return null;

        var periodo = await _periodoRepository.ObtenerPorId(idPeriodo, curso.IdCicloLectivo);
        if (periodo is null) return null;

        var calificaciones = await _calificacionRepository
            .ObtenerPorCursoMateriaYPeriodo(idCurso, idMateria, idPeriodo);

        return new CalificacionPorCursoResponseDto
        {
            IdCurso            = idCurso,
            IdMateria          = idMateria,
            NombreMateria      = materia.Nombre,
            IdPeriodoEvaluacion = idPeriodo,
            NombrePeriodo      = periodo.Nombre,
            Calificaciones     = calificaciones.Select(MapearAResponseDto).ToList()
        };
    }

    public async Task<List<CalificacionResponseDto>> ObtenerPorAlumno(
        int idAlumno, int idPeriodo, int idEscuela)
    {
        var calificaciones = await _calificacionRepository.ObtenerPorAlumno(idAlumno, idPeriodo);
        return calificaciones.Select(MapearAResponseDto).ToList();
    }

    public async Task<(bool exito, string mensaje)> Registrar(CalificacionRegistrarDto dto, int idEscuela)
    {
        // Validaciones
        var curso = await _cursoRepository.ObtenerPorId(dto.IdCurso, idEscuela);
        if (curso is null)
            return (false, "El curso no existe.");

        var materia = await _materiaRepository.ObtenerPorId(dto.IdMateria, idEscuela);
        if (materia is null)
            return (false, "La materia no existe.");

        var periodo = await _periodoRepository.ObtenerPorId(dto.IdPeriodoEvaluacion, curso.IdCicloLectivo);
        if (periodo is null)
            return (false, "El período de evaluación no existe.");

        if (!periodo.Activo)
            return (false, "El período de evaluación no está activo.");

        if (!dto.Alumnos.Any())
            return (false, "Debe incluir al menos un alumno.");

        // Validar rango de calificaciones (1 a 10)
        var fueraDeRango = dto.Alumnos.Where(a => a.ValorCalificacion < 1 || a.ValorCalificacion > 10).ToList();
        if (fueraDeRango.Any())
            return (false, "Las calificaciones deben estar entre 1 y 10.");

        // Para cada alumno: actualizar si ya existe, crear si no
        var nuevas     = new List<Calificacion>();
        var existentes = new List<Calificacion>();

        foreach (var item in dto.Alumnos)
        {
            var existente = await _calificacionRepository
                .ObtenerPorAlumnoMateriaYPeriodo(item.IdAlumno, dto.IdMateria, dto.IdPeriodoEvaluacion);

            if (existente is not null)
            {
                existente.ValorCalificacion = item.ValorCalificacion;
                existente.Observacion       = item.Observacion;
                existente.Fecha             = DateTime.Now;
                existente.FechaAct          = DateTime.Now;
                existentes.Add(existente);
            }
            else
            {
                nuevas.Add(new Calificacion
                {
                    IdAlumno            = item.IdAlumno,
                    IdMateria           = dto.IdMateria,
                    IdPeriodoEvaluacion = dto.IdPeriodoEvaluacion,
                    ValorCalificacion   = item.ValorCalificacion,
                    Observacion         = item.Observacion,
                    Fecha               = DateTime.Now,
                    Activo              = true
                });
            }
        }

        if (nuevas.Any())
            await _calificacionRepository.RegistrarLote(nuevas);

        if (existentes.Any())
            await _calificacionRepository.ActualizarLote(existentes);

        return (true, $"Se registraron {nuevas.Count} calificaciones nuevas y se actualizaron {existentes.Count}.");
    }

    private static CalificacionResponseDto MapearAResponseDto(Calificacion c) => new()
    {
        IdCalificacion      = c.IdCalificacion,
        IdAlumno            = c.IdAlumno,
        NombreAlumno        = c.Alumno.Nombre,
        ApellidoAlumno      = c.Alumno.Apellido,
        IdMateria           = c.IdMateria,
        NombreMateria       = c.Materia.Nombre,
        IdPeriodoEvaluacion = c.IdPeriodoEvaluacion,
        NombrePeriodo       = c.PeriodoEvaluacion.Nombre,
        ValorCalificacion   = c.ValorCalificacion,
        Observacion         = c.Observacion,
        Fecha               = c.Fecha
    };
}