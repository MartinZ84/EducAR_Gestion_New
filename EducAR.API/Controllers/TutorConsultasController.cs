using EducAR.API.Data;
using EducAR.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/tutor")]
[Authorize(Roles = "Tutor")]
public class TutorConsultasController : ControllerBase
{
    private readonly AppDbContext _context;

    public TutorConsultasController(AppDbContext context) => _context = context;

    private int IdUsuarioActual => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private int IdEscuelaActual => int.Parse(User.FindFirstValue("IdEscuela")!);

    private IQueryable<AlumnoTutor> MisRelaciones => _context.AlumnoTutores.Where(r =>
        r.Activo && r.Tutor.IdUsuario == IdUsuarioActual &&
        r.Tutor.Usuario.Activo && r.Tutor.Usuario.IdEscuela == IdEscuelaActual &&
        r.Alumno.IdEscuela == IdEscuelaActual);

    [HttpGet("mis-alumnos")]
    public async Task<IActionResult> ObtenerMisAlumnos()
    {
        var alumnos = await MisRelaciones
            .Select(r => new
            {
                r.IdAlumno,
                r.Alumno.Nombre,
                r.Alumno.Apellido
            })
            .ToListAsync();
        return Ok(alumnos);
    }

    [HttpGet("alumnos/{idAlumno}/asistencias")]
    public async Task<IActionResult> ObtenerAsistencias(int idAlumno)
    {
        if (!await MisRelaciones.AnyAsync(r => r.IdAlumno == idAlumno)) return NotFound();
        var asistencias = await _context.Asistencias
            .Where(a => a.IdAlumno == idAlumno && a.Activo && a.Curso.IdEscuela == IdEscuelaActual)
            .OrderByDescending(a => a.Fecha)
            .Select(a => new
            {
                a.IdAsistencia,
                a.IdCurso,
                a.Fecha,
                a.Presente,
                a.Curso.Grado,
                a.Curso.Division,
                Anio = a.Curso.CicloLectivo.Anio
            })
            .ToListAsync();
        return Ok(asistencias);
    }

    [HttpGet("alumnos/{idAlumno}/calificaciones")]
    public async Task<IActionResult> ObtenerCalificaciones(int idAlumno)
    {
        if (!await MisRelaciones.AnyAsync(r => r.IdAlumno == idAlumno)) return NotFound();
        var calificaciones = await _context.Calificaciones
            .Where(c => c.IdAlumno == idAlumno && c.Activo && c.Materia.IdEscuela == IdEscuelaActual)
            .OrderByDescending(c => c.PeriodoEvaluacion.CicloLectivo.Anio)
            .ThenBy(c => c.Materia.Nombre)
            .Select(c => new
            {
                c.IdCalificacion,
                c.Materia.Nombre,
                Periodo = c.PeriodoEvaluacion.Nombre,
                Anio = c.PeriodoEvaluacion.CicloLectivo.Anio,
                Nota = c.ValorCalificacion,
                c.Observacion,
                c.Fecha
            })
            .ToListAsync();
        return Ok(calificaciones);
    }

    [HttpGet("alumnos/{idAlumno}/notas")]
    public async Task<IActionResult> ObtenerNotas(int idAlumno)
    {
        if (!await MisRelaciones.AnyAsync(r => r.IdAlumno == idAlumno)) return NotFound();
        var notas = await _context.NotasEvaluacion
            .Where(n => n.IdAlumno == idAlumno && n.Evaluacion.Activo &&
                        n.Evaluacion.Curso.IdEscuela == IdEscuelaActual)
            .OrderByDescending(n => n.Evaluacion.Fecha)
            .Select(n => new
            {
                n.IdNotaEvaluacion,
                n.Evaluacion.Titulo,
                n.Evaluacion.Fecha,
                Materia = n.Evaluacion.Materia.Nombre,
                Periodo = n.Evaluacion.PeriodoEvaluacion.Nombre,
                Anio = n.Evaluacion.Curso.CicloLectivo.Anio,
                n.Valor
            })
            .ToListAsync();
        return Ok(notas);
    }
}
