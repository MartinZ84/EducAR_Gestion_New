using EducAR.API.DTOs.PeriodosEvaluacion;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/ciclos/{idCicloLectivo}/periodos")]
[Authorize]
public class PeriodosEvaluacionController : ControllerBase
{
    private readonly IPeriodoEvaluacionService _periodoService;

    public PeriodosEvaluacionController(IPeriodoEvaluacionService periodoService)
    {
        _periodoService = periodoService;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    [HttpGet]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerTodos(int idCicloLectivo)
    {
        var periodos = await _periodoService.ObtenerTodos(idCicloLectivo, IdEscuelaActual);
        return Ok(periodos);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorId(int idCicloLectivo, int id)
    {
        var periodo = await _periodoService.ObtenerPorId(id, idCicloLectivo, IdEscuelaActual);
        if (periodo is null) return NotFound(new { mensaje = "Período no encontrado." });
        return Ok(periodo);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear(int idCicloLectivo, [FromBody] PeriodoEvaluacionCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        dto.IdCicloLectivo = idCicloLectivo;
        var (exito, mensaje, periodo) = await _periodoService.Crear(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return CreatedAtAction(nameof(ObtenerPorId),
            new { idCicloLectivo, id = periodo!.IdPeriodoEvaluacion }, periodo);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int idCicloLectivo, int id, [FromBody] PeriodoEvaluacionUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje) = await _periodoService.Actualizar(id, idCicloLectivo, IdEscuelaActual, dto);
        if (!exito) return NotFound(new { mensaje });

        return Ok(new { mensaje });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int idCicloLectivo, int id)
    {
        var (exito, mensaje) = await _periodoService.Eliminar(id, idCicloLectivo, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return Ok(new { mensaje });
    }
}