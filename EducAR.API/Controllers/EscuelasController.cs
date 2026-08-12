using EducAR.API.DTOs.Escuelas;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EscuelasController : ControllerBase
{
    private readonly IEscuelaService _escuelaService;

    public EscuelasController(IEscuelaService escuelaService)
    {
        _escuelaService = escuelaService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas([FromQuery] FiltroNombreDto filtro)
    {
        var resultado = await _escuelaService.ObtenerTodasPaginado(filtro);
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var escuela = await _escuelaService.ObtenerPorId(id);
        if (escuela is null) return NotFound(new { mensaje = "Escuela no encontrada." });
        return Ok(escuela);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] EscuelaCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje, escuela) = await _escuelaService.Crear(dto);
        if (!exito) return BadRequest(new { mensaje });

        return CreatedAtAction(nameof(ObtenerPorId), new { id = escuela!.IdEscuela }, escuela);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] EscuelaUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje) = await _escuelaService.Actualizar(id, dto);
        if (!exito) return NotFound(new { mensaje });

        return Ok(new { mensaje });
    }
}
