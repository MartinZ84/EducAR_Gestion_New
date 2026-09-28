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
    private readonly AppDbContext _context;

    public BoletinService(
        IBoletinRepository boletinRepository,
        ICursoRepository cursoRepository,
        IPeriodoEvaluacionRepository periodoRepository,
        AppDbContext context)
    {
        _boletinRepository       = boletinRepository;
        _cursoRepository         = cursoRepository;
        _periodoRepository       = periodoRepository;
        _context                 = context;
    }

    public Task<List<BoletinResponseDto>> ObtenerPorCursoYPeriodo(int idCurso, int idPeriodo, int idEscuela) =>
        CalcularPorCursoYPeriodo(idCurso, idPeriodo, idEscuela);

    public async Task<BoletinResponseDto?> ObtenerPorAlumno(int idAlumno, int idCurso, int idPeriodo, int idEscuela) =>
        (await CalcularPorCursoYPeriodo(idCurso, idPeriodo, idEscuela))
            .FirstOrDefault(b => b.IdAlumno == idAlumno);

    public async Task<(bool exito, string mensaje, int boletinesGenerados)> Generar(BoletinGenerarDto dto, int idEscuela)
    {
        var curso = await _cursoRepository.ObtenerPorId(dto.IdCurso, idEscuela);
        if (curso is null) return (false, "El curso no existe.", 0);
        if (await _periodoRepository.ObtenerPorId(dto.IdPeriodoEvaluacion, curso.IdCicloLectivo) is null)
            return (false, "El período de evaluación no pertenece al ciclo lectivo del curso.", 0);

        var calculados = await CalcularPorCursoYPeriodo(dto.IdCurso, dto.IdPeriodoEvaluacion, idEscuela);
        if (calculados.Count == 0) return (false, "El curso no tiene alumnos con matrícula activa.", 0);

        var nuevos = 0;
        foreach (var boletin in calculados)
            if (await GuardarCalculo(boletin)) nuevos++;

        return (true, $"Se generaron o actualizaron {calculados.Count} boletines.", nuevos);
    }

    public async Task<(bool exito, string mensaje, BoletinResponseDto? boletin)> GenerarParaAlumno(
        int idAlumno, int idCurso, int idPeriodo, int idEscuela)
    {
        var curso = await _cursoRepository.ObtenerPorId(idCurso, idEscuela);
        if (curso is null) return (false, "El curso no existe.", null);
        if (await _periodoRepository.ObtenerPorId(idPeriodo, curso.IdCicloLectivo) is null)
            return (false, "El período no pertenece al ciclo lectivo del curso.", null);

        var calculado = (await CalcularPorCursoYPeriodo(idCurso, idPeriodo, idEscuela))
            .FirstOrDefault(b => b.IdAlumno == idAlumno);
        if (calculado is null)
            return (false, "El alumno no tiene matrícula activa en este curso y ciclo lectivo.", null);

        await GuardarCalculo(calculado);
        var generado = (await CalcularPorCursoYPeriodo(idCurso, idPeriodo, idEscuela))
            .FirstOrDefault(b => b.IdAlumno == idAlumno);
        return generado is null
            ? (false, "No se pudo recuperar el boletín generado.", null)
            : (true, "Boletín actualizado.", generado);
    }

    private async Task<bool> GuardarCalculo(BoletinResponseDto calculado)
    {
        var existente = await _boletinRepository.ObtenerPorAlumnoCursoYPeriodo(
            calculado.IdAlumno, calculado.IdCurso, calculado.IdPeriodoEvaluacion);
        var detalles = calculado.Detalle.Select(d => new DetalleBoletin
        {
            IdMateria = d.IdMateria,
            CalificacionFinal = d.CalificacionFinal,
            ConceptoFinal = d.ConceptoFinal,
            Activo = true
        }).ToList();

        if (existente is null)
        {
            await _boletinRepository.Crear(new Boletin
            {
                IdAlumno = calculado.IdAlumno,
                IdCurso = calculado.IdCurso,
                IdPeriodoEvaluacion = calculado.IdPeriodoEvaluacion,
                Activo = true,
                DetallesBoletines = detalles
            });
            return true;
        }

        existente.Activo = true;
        existente.DetallesBoletines = detalles;
        await _boletinRepository.Actualizar(existente, reemplazarDetalles: true);
        return false;
    }

    private async Task<List<BoletinResponseDto>> CalcularPorCursoYPeriodo(int idCurso, int idPeriodo, int idEscuela)
    {
        var curso = await _cursoRepository.ObtenerPorId(idCurso, idEscuela);
        if (curso is null) return new();
        var periodo = await _periodoRepository.ObtenerPorId(idPeriodo, curso.IdCicloLectivo);
        if (periodo is null) return new();

        var matriculas = await _context.Matriculas.AsNoTracking()
            .Where(m => m.IdCurso == idCurso && m.IdCicloLectivo == curso.IdCicloLectivo &&
                        m.IdEscuela == idEscuela && m.Estado == EstadoMatricula.Activa &&
                        m.Alumno.Activo && m.Alumno.IdEscuela == idEscuela)
            .Select(m => new
            {
                m.IdAlumno,
                m.Alumno.Dni,
                m.Alumno.Nombre,
                m.Alumno.Apellido
            })
            .OrderBy(m => m.Apellido).ThenBy(m => m.Nombre)
            .ToListAsync();
        if (matriculas.Count == 0) return new();

        var materias = await _context.DocenteMateriaCursos.AsNoTracking()
            .Where(a => a.IdCurso == idCurso && a.Activo && a.Materia.Activo &&
                        a.Materia.IdEscuela == idEscuela)
            .Select(a => new { a.IdMateria, Nombre = a.Materia.Nombre })
            .Distinct().OrderBy(m => m.Nombre).ToListAsync();

        var idAlumnos = matriculas.Select(m => m.IdAlumno).ToList();
        var promediosConsulta = await _context.NotasEvaluacion.AsNoTracking()
            .Where(n => idAlumnos.Contains(n.IdAlumno) && n.Alumno.Activo &&
                        n.Alumno.IdEscuela == idEscuela && n.Evaluacion.Activo &&
                        n.Evaluacion.IdCurso == idCurso &&
                        n.Evaluacion.IdPeriodoEvaluacion == idPeriodo &&
                        n.Evaluacion.Materia.Activo && n.Evaluacion.Materia.IdEscuela == idEscuela)
            .GroupBy(n => new { n.IdAlumno, n.Evaluacion.IdMateria })
            .Select(g => new
            {
                g.Key.IdAlumno,
                g.Key.IdMateria,
                Promedio = g.Average(n => n.Valor)
            })
            .ToListAsync();
        var promedios = promediosConsulta.ToDictionary(x => (x.IdAlumno, x.IdMateria));

        var boletinesGuardados = await _context.Boletines.AsNoTracking()
            .Include(b => b.DetallesBoletines)
            .Where(b => b.IdCurso == idCurso && b.IdPeriodoEvaluacion == idPeriodo && idAlumnos.Contains(b.IdAlumno))
            .ToDictionaryAsync(b => b.IdAlumno);

        var respuesta = new List<BoletinResponseDto>(matriculas.Count);
        foreach (var matricula in matriculas)
        {
            boletinesGuardados.TryGetValue(matricula.IdAlumno, out var guardado);
            var detalle = materias.Select(materia =>
            {
                promedios.TryGetValue((matricula.IdAlumno, materia.IdMateria), out var agregado);
                var nota = agregado is null
                    ? (decimal?)null
                    : Math.Round(agregado.Promedio, 2, MidpointRounding.AwayFromZero);
                return new DetalleBoletinResponseDto
                {
                    IdMateria = materia.IdMateria,
                    NombreMateria = materia.Nombre,
                    CalificacionFinal = nota,
                    ConceptoFinal = nota.HasValue ? ObtenerConcepto(nota.Value) : null
                };
            }).ToList();
            var notasConCalificacion = detalle.Where(d => d.CalificacionFinal.HasValue)
                .Select(d => d.CalificacionFinal!.Value).ToList();
            var detalleGuardado = guardado?.DetallesBoletines.ToDictionary(d => d.IdMateria);
            var requiereRegeneracion = guardado is null || !guardado.Activo || detalle.Count != (detalleGuardado?.Count ?? 0) ||
                detalle.Any(d => detalleGuardado is null || !detalleGuardado.TryGetValue(d.IdMateria, out var anterior) ||
                                 anterior.CalificacionFinal != d.CalificacionFinal || anterior.Activo != true);

            respuesta.Add(new BoletinResponseDto
            {
                IdBoletin = guardado?.IdBoletin ?? 0,
                IdAlumno = matricula.IdAlumno,
                DniAlumno = matricula.Dni,
                NombreAlumno = matricula.Nombre,
                ApellidoAlumno = matricula.Apellido,
                IdCurso = idCurso,
                Curso = $"{curso.Grado}° {curso.Division} - {curso.CicloLectivo.Anio}",
                IdCicloLectivo = curso.IdCicloLectivo,
                AnioLectivo = curso.CicloLectivo.Anio,
                IdPeriodoEvaluacion = idPeriodo,
                NombrePeriodo = periodo.Nombre,
                FechaInicioPeriodo = periodo.FechaInicio,
                FechaFinPeriodo = periodo.FechaFin,
                ObservacionGeneral = guardado?.ObservacionGeneral,
                PromedioGeneral = notasConCalificacion.Count == 0
                    ? null
                    : Math.Round(notasConCalificacion.Average(), 2, MidpointRounding.AwayFromZero),
                FechaGeneracion = guardado?.FechaCrea,
                EstaGuardado = guardado is { Activo: true },
                RequiereRegeneracion = requiereRegeneracion,
                Detalle = detalle
            });
        }

        return respuesta;
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
        >= 9.5m => "Sobresaliente",
        >= 9m => "Excelente",
        >= 8m => "Muy bueno",
        >= 7m => "Bueno",
        _    => "Insuficiente"
    };

}