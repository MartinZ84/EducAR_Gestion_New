using EducAR.API.DTOs.AlumnoTutor;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlumnoTutorController : ControllerBase
{
    private readonly IAlumnoTutorService _alumnoTutorService;

    public AlumnoTutorController(IAlumnoTutorService alumnoTutorService)
    {
        _alumnoTutorService = alumnoTutorService;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    // GET api/alumnotutor/alumno/1
    [HttpGet("alumno/{idAlumno}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorAlumno(int idAlumno)
    {
        var relaciones = await _alumnoTutorService.ObtenerPorAlumno(idAlumno, IdEscuelaActual);
        return Ok(relaciones);
    }

    // GET api/alumnotutor/tutor/1
    [HttpGet("tutor/{idTutor}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorTutor(int idTutor)
    {
        var relaciones = await _alumnoTutorService.ObtenerPorTutor(idTutor, IdEscuelaActual);
        return Ok(relaciones);
    }

    // POST api/alumnotutor
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Asociar([FromBody] AlumnoTutorCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje, relacion) = await _alumnoTutorService.Asociar(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return Ok(relacion);
    }

    // DELETE api/alumnotutor/1
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Desasociar(int id)
    {
        var (exito, mensaje) = await _alumnoTutorService.Desasociar(id, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return Ok(new { mensaje });
    }

    // GET api/alumnotutor/mis-alumnos
    [HttpGet("mis-alumnos")]
    [Authorize(Roles = "Tutor")]
    public async Task<IActionResult> ObtenerMisAlumnos()
    {
        var idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var alumnos = await _alumnoTutorService.ObtenerMisAlumnos(idUsuario, IdEscuelaActual);
        return Ok(alumnos);
    }
}