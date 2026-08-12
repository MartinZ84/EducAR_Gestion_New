using EducAR.API.DTOs.Alumnos;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlumnosController : ControllerBase
{
    private readonly IAlumnoService _alumnoService;

    public AlumnosController(IAlumnoService alumnoService)
    {
        _alumnoService = alumnoService;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    [HttpGet]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerTodos([FromQuery] FiltroPersonaDto filtro)
    {
        var resultado = await _alumnoService.ObtenerTodosPaginado(IdEscuelaActual, filtro);
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var alumno = await _alumnoService.ObtenerPorId(id, IdEscuelaActual);
        if (alumno is null) return NotFound(new { mensaje = "Alumno no encontrado." });
        return Ok(alumno);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] AlumnoCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje, alumno) = await _alumnoService.Crear(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return CreatedAtAction(nameof(ObtenerPorId), new { id = alumno!.IdAlumno }, alumno);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] AlumnoUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje) = await _alumnoService.Actualizar(id, IdEscuelaActual, dto);
        if (!exito) return NotFound(new { mensaje });

        return Ok(new { mensaje });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var exito = await _alumnoService.Eliminar(id, IdEscuelaActual);
        if (!exito) return NotFound(new { mensaje = "Alumno no encontrado." });

        return Ok(new { mensaje = "Alumno dado de baja correctamente." });
    }

    // ==========================
    // TUTORES DEL ALUMNO
    // ==========================

    [HttpPost("{id}/tutores")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> AsociarTutor(int id, [FromBody] AsociarTutorDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje) = await _alumnoService.AsociarTutor(id, IdEscuelaActual, dto);
        if (!exito)
        {
            if (mensaje.Contains("no encontrado")) return NotFound(new { mensaje });
            return BadRequest(new { mensaje });
        }

        return Ok(new { mensaje });
    }

    [HttpDelete("{id}/tutores/{idTutor}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> QuitarTutor(int id, int idTutor)
    {
        var (exito, mensaje) = await _alumnoService.QuitarTutor(id, IdEscuelaActual, idTutor);
        if (!exito) return NotFound(new { mensaje });

        return Ok(new { mensaje });
    }
}
