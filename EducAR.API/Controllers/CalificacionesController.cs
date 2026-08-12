using EducAR.API.DTOs.Calificaciones;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CalificacionesController : ControllerBase
{
    private readonly ICalificacionService _calificacionService;

    public CalificacionesController(ICalificacionService calificacionService)
    {
        _calificacionService = calificacionService;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    // GET api/calificaciones/curso/1/materia/2/periodo/1
    [HttpGet("curso/{idCurso}/materia/{idMateria}/periodo/{idPeriodo}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorCursoMateriaYPeriodo(
        int idCurso, int idMateria, int idPeriodo)
    {
        var resultado = await _calificacionService
            .ObtenerPorCursoMateriaYPeriodo(idCurso, idMateria, idPeriodo, IdEscuelaActual);

        if (resultado is null)
            return NotFound(new { mensaje = "Curso, materia o período no encontrado." });

        return Ok(resultado);
    }

    // GET api/calificaciones/alumno/1/periodo/1
    [HttpGet("alumno/{idAlumno}/periodo/{idPeriodo}")]
    [Authorize(Roles = "Administrador,Docente,Tutor")]
    public async Task<IActionResult> ObtenerPorAlumno(int idAlumno, int idPeriodo)
    {
        var resultado = await _calificacionService
            .ObtenerPorAlumno(idAlumno, idPeriodo, IdEscuelaActual);

        return Ok(resultado);
    }

    // POST api/calificaciones
    [HttpPost]
    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> Registrar([FromBody] CalificacionRegistrarDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje) = await _calificacionService.Registrar(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return Ok(new { mensaje });
    }
}