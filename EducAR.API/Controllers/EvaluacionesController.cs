using EducAR.API.Data;
using EducAR.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace EducAR.API.Controllers;

public class EvaluacionCrearDto
{
    [Range(1, int.MaxValue)] public int IdCurso { get; set; }
    [Range(1, int.MaxValue)] public int IdMateria { get; set; }
    [Range(1, int.MaxValue)] public int IdPeriodoEvaluacion { get; set; }
    [Required, MaxLength(200)] public string Titulo { get; set; } = null!;
    [Required] public DateTime Fecha { get; set; }
}

public class NotaEvaluacionDto
{
    [Range(1, int.MaxValue)] public int IdAlumno { get; set; }
    [Range(1, 10)] public decimal? Valor { get; set; }
}

public class NotasEvaluacionGuardarDto
{
    [Required] public List<NotaEvaluacionDto> Notas { get; set; } = new();
}

[ApiController]
[Route("api/evaluaciones")]
[Authorize(Roles = "Docente")]
public class EvaluacionesController : ControllerBase
{
    private readonly AppDbContext _context;
    public EvaluacionesController(AppDbContext context) => _context = context;

    private int IdUsuarioActual => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private int IdEscuelaActual => int.Parse(User.FindFirstValue("IdEscuela")!);

    private Task<bool> EstaAsignado(int idCurso, int idMateria) => _context.DocenteMateriaCursos
        .AnyAsync(a => a.IdCurso == idCurso && a.IdMateria == idMateria && a.Activo &&
                       a.Docente.IdUsuario == IdUsuarioActual && a.Curso.IdEscuela == IdEscuelaActual);

    [HttpGet("curso/{idCurso}/materia/{idMateria}/periodo/{idPeriodo}")]
    public async Task<IActionResult> Obtener(int idCurso, int idMateria, int idPeriodo)
    {
        if (!await EstaAsignado(idCurso, idMateria)) return Forbid();
        var evaluaciones = await _context.Evaluaciones
            .Where(e => e.IdCurso == idCurso && e.IdMateria == idMateria &&
                        e.IdPeriodoEvaluacion == idPeriodo && e.Activo)
            .OrderBy(e => e.Fecha)
            .Select(e => new
            {
                e.IdEvaluacion,
                e.Titulo,
                e.Fecha,
                Notas = e.Notas.Select(n => new { n.IdAlumno, n.Valor }).ToList()
            })
            .ToListAsync();
        return Ok(evaluaciones);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] EvaluacionCrearDto dto)
    {
        if (!await EstaAsignado(dto.IdCurso, dto.IdMateria)) return Forbid();
        var periodo = await _context.PeriodosEvaluacion.FirstOrDefaultAsync(p =>
            p.IdPeriodoEvaluacion == dto.IdPeriodoEvaluacion && p.Activo &&
            p.CicloLectivo.Cursos.Any(c => c.IdCurso == dto.IdCurso));
        if (periodo is null) return BadRequest(new { mensaje = "El período no corresponde al curso." });
        if (dto.Fecha.Date < periodo.FechaInicio.Date || dto.Fecha.Date > periodo.FechaFin.Date)
            return BadRequest(new { mensaje = "La fecha debe estar dentro del período." });

        var evaluacion = new Evaluacion
        {
            IdCurso = dto.IdCurso,
            IdMateria = dto.IdMateria,
            IdPeriodoEvaluacion = dto.IdPeriodoEvaluacion,
            Titulo = dto.Titulo.Trim(),
            Fecha = dto.Fecha.Date
        };
        _context.Evaluaciones.Add(evaluacion);
        await _context.SaveChangesAsync();
        return Ok(new { evaluacion.IdEvaluacion });
    }

    [HttpPut("{idEvaluacion}/notas")]
    public async Task<IActionResult> GuardarNotas(int idEvaluacion, [FromBody] NotasEvaluacionGuardarDto dto)
    {
        var evaluacion = await _context.Evaluaciones.FirstOrDefaultAsync(e => e.IdEvaluacion == idEvaluacion && e.Activo);
        if (evaluacion is null) return NotFound();
        if (!await EstaAsignado(evaluacion.IdCurso, evaluacion.IdMateria)) return Forbid();
        if (dto.Notas.Select(n => n.IdAlumno).Distinct().Count() != dto.Notas.Count)
            return BadRequest(new { mensaje = "Un alumno no puede tener notas duplicadas." });
        if (dto.Notas.Any(n => n.Valor is < 1 or > 10))
            return BadRequest(new { mensaje = "Las notas deben estar entre 1 y 10." });

        var idsMatriculados = await _context.Matriculas
            .Where(m => m.IdCurso == evaluacion.IdCurso && m.Estado == EstadoMatricula.Activa)
            .Select(m => m.IdAlumno).ToListAsync();
        if (dto.Notas.Any(n => !idsMatriculados.Contains(n.IdAlumno)))
            return BadRequest(new { mensaje = "Hay alumnos que no pertenecen al curso." });

        var existentes = await _context.NotasEvaluacion
            .Where(n => n.IdEvaluacion == idEvaluacion).ToListAsync();
        foreach (var nota in dto.Notas)
        {
            var existente = existentes.FirstOrDefault(n => n.IdAlumno == nota.IdAlumno);
            if (nota.Valor is null)
            {
                if (existente is not null) _context.NotasEvaluacion.Remove(existente);
            }
            else if (existente is null)
                _context.NotasEvaluacion.Add(new NotaEvaluacion
                    { IdEvaluacion = idEvaluacion, IdAlumno = nota.IdAlumno, Valor = nota.Valor.Value });
            else
            {
                existente.Valor = nota.Valor.Value;
                existente.FechaAct = DateTime.Now;
            }
        }

        await using var transaccion = _context.Database.IsRelational()
            ? await _context.Database.BeginTransactionAsync() : null;
        await _context.SaveChangesAsync();

        var alumnosConNotas = await _context.NotasEvaluacion
            .Where(n => n.Evaluacion.IdCurso == evaluacion.IdCurso &&
                        n.Evaluacion.IdMateria == evaluacion.IdMateria &&
                        n.Evaluacion.IdPeriodoEvaluacion == evaluacion.IdPeriodoEvaluacion &&
                        n.Evaluacion.Activo)
            .GroupBy(n => n.IdAlumno)
            .Select(g => new { IdAlumno = g.Key, Promedio = g.Average(n => n.Valor) })
            .ToListAsync();

        var idsAfectados = dto.Notas.Select(n => n.IdAlumno).ToList();
        var finales = await _context.Calificaciones.Where(c =>
            c.IdMateria == evaluacion.IdMateria && c.IdPeriodoEvaluacion == evaluacion.IdPeriodoEvaluacion &&
            idsAfectados.Contains(c.IdAlumno)).ToListAsync();
        foreach (var idAlumno in idsAfectados)
        {
            var alumno = alumnosConNotas.FirstOrDefault(a => a.IdAlumno == idAlumno);
            var final = finales.FirstOrDefault(c => c.IdAlumno == idAlumno);
            if (alumno is null)
            {
                if (final is not null) final.Activo = false;
                continue;
            }
            var promedio = Math.Round(alumno.Promedio, 2, MidpointRounding.AwayFromZero);
            if (final is null)
                _context.Calificaciones.Add(new Calificacion
                {
                    IdAlumno = idAlumno,
                    IdMateria = evaluacion.IdMateria,
                    IdPeriodoEvaluacion = evaluacion.IdPeriodoEvaluacion,
                    ValorCalificacion = promedio
                });
            else
            {
                final.ValorCalificacion = promedio;
                final.Activo = true;
                final.FechaAct = DateTime.Now;
            }
        }
        await _context.SaveChangesAsync();
        if (transaccion is not null) await transaccion.CommitAsync();
        return Ok(new { mensaje = "Notas y promedios guardados." });
    }
}
