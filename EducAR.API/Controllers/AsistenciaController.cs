using EducAR.API.DTOs.Asistencia;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AsistenciaController : ControllerBase
{
    private readonly IAsistenciaService _asistenciaService;

    public AsistenciaController(IAsistenciaService asistenciaService)
    {
        _asistenciaService = asistenciaService;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    private int IdUsuarioActual =>
        int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);

    // GET api/asistencia/curso/1?fecha=2026-07-01
    [HttpGet("curso/{idCurso}")]
    [Authorize(Roles = "Administrador,Docente,Tutor")]
    public async Task<IActionResult> ObtenerPorCursoYFecha(int idCurso, [FromQuery] DateTime fecha)
    {
        var resultado = await _asistenciaService.ObtenerPorCursoYFecha(idCurso, fecha, IdEscuelaActual);
        if (resultado is null) return NotFound(new { mensaje = "Curso no encontrado." });
        return Ok(resultado);
    }

    // GET api/asistencia/alumno/1/curso/1
    [HttpGet("alumno/{idAlumno}/curso/{idCurso}")]
    [Authorize(Roles = "Administrador,Docente,Tutor")]
    public async Task<IActionResult> ObtenerResumenAlumno(int idAlumno, int idCurso)
    {
        var resultado = await _asistenciaService.ObtenerResumenAlumno(idAlumno, idCurso, IdEscuelaActual);
        return Ok(resultado);
    }

    // POST api/asistencia
    [HttpPost]
    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> Registrar([FromBody] AsistenciaRegistrarDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Obtener el IdDocente desde el IdUsuario del token
        var idUsuario = IdUsuarioActual;
        var (exito, mensaje) = await _asistenciaService.Registrar(dto, idUsuario, IdEscuelaActual);

        if (!exito) return BadRequest(new { mensaje });
        return Ok(new { mensaje });
    }
}