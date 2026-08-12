using EducAR.API.DTOs.CiclosLectivos;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CiclosLectivosController : ControllerBase
{
    private readonly ICicloLectivoService _cicloLectivoService;

    public CiclosLectivosController(ICicloLectivoService cicloLectivoService)
    {
        _cicloLectivoService = cicloLectivoService;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    [HttpGet]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerTodos()
    {
        var ciclos = await _cicloLectivoService.ObtenerTodos(IdEscuelaActual);
        return Ok(ciclos);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var ciclo = await _cicloLectivoService.ObtenerPorId(id, IdEscuelaActual);
        if (ciclo is null) return NotFound(new { mensaje = "Ciclo lectivo no encontrado." });
        return Ok(ciclo);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] CicloLectivoCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje, ciclo) = await _cicloLectivoService.Crear(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return CreatedAtAction(nameof(ObtenerPorId), new { id = ciclo!.IdCicloLectivo }, ciclo);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] CicloLectivoUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje) = await _cicloLectivoService.Actualizar(id, IdEscuelaActual, dto);
        if (!exito) return NotFound(new { mensaje });

        return Ok(new { mensaje });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var exito = await _cicloLectivoService.Eliminar(id, IdEscuelaActual);
        if (!exito) return NotFound(new { mensaje = "Ciclo lectivo no encontrado." });

        return Ok(new { mensaje = "Ciclo lectivo dado de baja correctamente." });
    }
}