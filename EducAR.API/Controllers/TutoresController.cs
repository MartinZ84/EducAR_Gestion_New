using EducAR.API.DTOs.Tutores;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TutoresController : ControllerBase
{
    private readonly ITutorService _tutorService;

    public TutoresController(ITutorService tutorService)
    {
        _tutorService = tutorService;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    [HttpGet]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerTodos([FromQuery] FiltroPersonaDto filtro)
    {
        var resultado = await _tutorService.ObtenerTodosPaginado(IdEscuelaActual, filtro);
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var tutor = await _tutorService.ObtenerPorId(id, IdEscuelaActual);
        if (tutor is null) return NotFound(new { mensaje = "Tutor no encontrado." });
        return Ok(tutor);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] TutorCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje, tutor) = await _tutorService.Crear(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return CreatedAtAction(nameof(ObtenerPorId), new { id = tutor!.IdTutor }, tutor);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] TutorUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje) = await _tutorService.Actualizar(id, IdEscuelaActual, dto);
        if (!exito) return NotFound(new { mensaje });

        return Ok(new { mensaje });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var exito = await _tutorService.Eliminar(id, IdEscuelaActual);
        if (!exito) return NotFound(new { mensaje = "Tutor no encontrado." });

        return Ok(new { mensaje = "Tutor dado de baja correctamente." });
    }
}
