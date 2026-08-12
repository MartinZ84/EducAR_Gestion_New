using EducAR.API.DTOs.Boletines;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;
using EducAR.API.Data;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Services;

public class BoletinService : IBoletinService
{
    private readonly IBoletinRepository _boletinRepository;
    private readonly ICursoRepository _cursoRepository;
    private readonly IPeriodoEvaluacionRepository _periodoRepository;
    private readonly ICalificacionRepository _calificacionRepository;
    private readonly AppDbContext _context;

    public BoletinService(
        IBoletinRepository boletinRepository,
        ICursoRepository cursoRepository,
        IPeriodoEvaluacionRepository periodoRepository,
        ICalificacionRepository calificacionRepository,
        AppDbContext context)
    {
        _boletinRepository       = boletinRepository;
        _cursoRepository         = cursoRepository;
        _periodoRepository       = periodoRepository;
        _calificacionRepository  = calificacionRepository;
        _context                 = context;
    }

    public async Task<List<BoletinResponseDto>> ObtenerPorCursoYPeriodo(int idCurso, int idPeriodo, int idEscuela)
    {
        var curso = await _cursoRepository.ObtenerPorId(idCurso, idEscuela);
        if (curso is null) return new List<BoletinResponseDto>();

        var boletines = await _boletinRepository.ObtenerPorCursoYPeriodo(idCurso, idPeriodo);
        return boletines.Select(MapearAResponseDto).ToList();
    }

    public async Task<BoletinResponseDto?> ObtenerPorAlumno(int idAlumno, int idCurso, int idPeriodo, int idEscuela)
    {
        var curso = await _cursoRepository.ObtenerPorId(idCurso, idEscuela);
        if (curso is null) return null;

        var boletin = await _boletinRepository.ObtenerPorAlumnoCursoYPeriodo(idAlumno, idCurso, idPeriodo);
        return boletin is null ? null : MapearAResponseDto(boletin);
    }

    public async Task<(bool exito, string mensaje, int boletinesGenerados)> Generar(BoletinGenerarDto dto, int idEscuela)
    {
        // Validaciones
        var curso = await _cursoRepository.ObtenerPorId(dto.IdCurso, idEscuela);
        if (curso is null)
            return (false, "El curso no existe.", 0);

        var periodo = await _periodoRepository.ObtenerPorId(dto.IdPeriodoEvaluacion, curso.IdCicloLectivo);
        if (periodo is null)
            return (false, "El período de evaluación no existe.", 0);

        // Obtener alumnos activos del curso
        var alumnosCurso = await _context.Matriculas
            .Include(m => m.Alumno)
            .Where(m => m.IdCurso == dto.IdCurso && m.Estado == EstadoMatricula.Activa)
            .ToListAsync();

        if (!alumnosCurso.Any())
            return (false, "El curso no tiene alumnos inscriptos.", 0);

        int generados = 0;

        foreach (var matricula in alumnosCurso)
        {
            // Calificaciones del alumno en este período
            var calificaciones = await _calificacionRepository
                .ObtenerPorAlumno(matricula.IdAlumno, dto.IdPeriodoEvaluacion);

            if (!calificaciones.Any()) continue;

            // Si ya existe el boletín lo actualizamos, sino lo creamos
            var boletinExistente = await _boletinRepository
                .ObtenerPorAlumnoCursoYPeriodo(matricula.IdAlumno, dto.IdCurso, dto.IdPeriodoEvaluacion);

            if (boletinExistente is not null)
            {
                // Actualizar detalles existentes
                boletinExistente.DetallesBoletines.Clear();
                boletinExistente.DetallesBoletines = calificaciones.Select(c => new DetalleBoletin
                {
                    IdMateria         = c.IdMateria,
                    CalificacionFinal = c.ValorCalificacion,
                    ConceptoFinal     = ObtenerConcepto(c.ValorCalificacion),
                    Activo            = true
                }).ToList();

                await _boletinRepository.Actualizar(boletinExistente);
            }
            else
            {
                var boletin = new Boletin
                {
                    IdAlumno            = matricula.IdAlumno,
                    IdCurso             = dto.IdCurso,
                    IdPeriodoEvaluacion = dto.IdPeriodoEvaluacion,
                    Activo              = true,
                    DetallesBoletines   = calificaciones.Select(c => new DetalleBoletin
                    {
                        IdMateria         = c.IdMateria,
                        CalificacionFinal = c.ValorCalificacion,
                        ConceptoFinal     = ObtenerConcepto(c.ValorCalificacion),
                        Activo            = true
                    }).ToList()
                };

                await _boletinRepository.Crear(boletin);
                generados++;
            }
        }

        return (true, $"Se generaron {generados} boletines correctamente.", generados);
    }

    public async Task<(bool exito, string mensaje)> ActualizarObservacion(int idBoletin, BoletinObservacionDto dto, int idEscuela)
    {
        var boletin = await _boletinRepository.ObtenerPorId(idBoletin);
        if (boletin is null)
            return (false, "Boletín no encontrado.");

        // Validar que el boletín pertenece a la escuela
        var curso = await _cursoRepository.ObtenerPorId(boletin.IdCurso, idEscuela);
        if (curso is null)
            return (false, "No tiene permisos para modificar este boletín.");

        boletin.ObservacionGeneral = dto.ObservacionGeneral;
        await _boletinRepository.Actualizar(boletin);

        return (true, "Observación actualizada correctamente.");
    }

    // Convierte nota numérica a concepto
    private static string ObtenerConcepto(decimal valor) => valor switch
    {
        >= 9 => "Sobresaliente",
        >= 7 => "Bueno",
        >= 6 => "Regular",
        _    => "Insuficiente"
    };

    private static BoletinResponseDto MapearAResponseDto(Boletin b) => new()
    {
        IdBoletin          = b.IdBoletin,
        IdAlumno           = b.IdAlumno,
        NombreAlumno       = b.Alumno.Nombre,
        ApellidoAlumno     = b.Alumno.Apellido,
        IdCurso            = b.IdCurso,
        Curso              = $"{b.Curso.Grado}° {b.Curso.Division} - {b.Curso.CicloLectivo.Anio}",
        IdPeriodoEvaluacion = b.IdPeriodoEvaluacion,
        NombrePeriodo      = b.PeriodoEvaluacion.Nombre,
        ObservacionGeneral = b.ObservacionGeneral,
        PromedioGeneral    = b.DetallesBoletines.Any()
            ? Math.Round(b.DetallesBoletines.Average(d => d.CalificacionFinal), 2)
            : 0,
        FechaGeneracion    = b.FechaCrea,
        Detalle            = b.DetallesBoletines.Select(d => new DetalleBoletinResponseDto
        {
            IdMateria         = d.IdMateria,
            NombreMateria     = d.Materia.Nombre,
            CalificacionFinal = d.CalificacionFinal,
            ConceptoFinal     = d.ConceptoFinal
        }).ToList()
    };
}