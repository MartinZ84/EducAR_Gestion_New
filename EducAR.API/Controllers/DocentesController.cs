using EducAR.API.DTOs.Docentes;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocentesController : ControllerBase
{
    private readonly IDocenteService _docenteService;

    public DocentesController(IDocenteService docenteService)
    {
        _docenteService = docenteService;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerTodos([FromQuery] FiltroPersonaDto filtro)
    {
        var resultado = await _docenteService.ObtenerTodosPaginado(IdEscuelaActual, filtro);
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var docente = await _docenteService.ObtenerPorId(id, IdEscuelaActual);
        if (docente is null) return NotFound(new { mensaje = "Docente no encontrado." });
        return Ok(docente);
    }

    [HttpGet("{id}/detalle")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerDetalle(int id)
    {
        var docente = await _docenteService.ObtenerDetalle(id, IdEscuelaActual);
        if (docente is null) return NotFound(new { mensaje = "Registro no encontrado" });
        return Ok(docente);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] DocenteCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje, docente) = await _docenteService.Crear(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return CreatedAtAction(nameof(ObtenerPorId), new { id = docente!.IdDocente }, docente);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] DocenteUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje) = await _docenteService.Actualizar(id, IdEscuelaActual, dto);
        if (!exito) return NotFound(new { mensaje });

        return Ok(new { mensaje });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var exito = await _docenteService.Eliminar(id, IdEscuelaActual);
        if (!exito) return NotFound(new { mensaje = "Docente no encontrado." });

        return Ok(new { mensaje = "Docente dado de baja correctamente." });
    }
}
