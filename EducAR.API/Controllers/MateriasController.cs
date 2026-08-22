using EducAR.API.DTOs.Materias;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MateriasController : ControllerBase
{
    private readonly IMateriaService _materiaService;

    public MateriasController(IMateriaService materiaService)
    {
        _materiaService = materiaService;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    [HttpGet]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerTodas([FromQuery] FiltroNombreDto filtro)
    {
        var resultado = await _materiaService.ObtenerTodasPaginado(IdEscuelaActual, filtro);
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var materia = await _materiaService.ObtenerPorId(id, IdEscuelaActual);
        if (materia is null) return NotFound(new { mensaje = "Materia no encontrada." });
        return Ok(materia);
    }

    [HttpGet("{id}/detalle")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerDetalle(int id)
    {
        var materia = await _materiaService.ObtenerDetalle(id, IdEscuelaActual);
        if (materia is null) return NotFound(new { mensaje = "Registro no encontrado" });
        return Ok(materia);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] MateriaCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje, materia) = await _materiaService.Crear(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return CreatedAtAction(nameof(ObtenerPorId), new { id = materia!.IdMateria }, materia);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] MateriaUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje) = await _materiaService.Actualizar(id, IdEscuelaActual, dto);
        if (!exito) return NotFound(new { mensaje });

        return Ok(new { mensaje });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var exito = await _materiaService.Eliminar(id, IdEscuelaActual);
        if (!exito) return NotFound(new { mensaje = "Materia no encontrada." });

        return Ok(new { mensaje = "Materia dada de baja correctamente." });
    }
}
