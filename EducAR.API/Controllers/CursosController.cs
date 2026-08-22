using EducAR.API.DTOs.Cursos;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CursosController : ControllerBase
{
    private readonly ICursoService _cursoService;

    public CursosController(ICursoService cursoService)
    {
        _cursoService = cursoService;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    [HttpGet]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerTodos([FromQuery] FiltroCursoDto filtro)
    {
        var resultado = await _cursoService.ObtenerTodosPaginado(IdEscuelaActual, filtro);
        return Ok(resultado);
    }

    [HttpGet("ciclo/{idCicloLectivo}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorCicloLectivo(int idCicloLectivo)
    {
        var cursos = await _cursoService.ObtenerPorCicloLectivo(idCicloLectivo, IdEscuelaActual);
        return Ok(cursos);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var curso = await _cursoService.ObtenerPorId(id, IdEscuelaActual);
        if (curso is null) return NotFound(new { mensaje = "Curso no encontrado." });
        return Ok(curso);
    }

    [HttpGet("{id}/detalle")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerDetalle(int id)
    {
        var curso = await _cursoService.ObtenerDetalle(id, IdEscuelaActual);
        if (curso is null) return NotFound(new { mensaje = "Registro no encontrado" });
        return Ok(curso);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] CursoCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje, curso) = await _cursoService.Crear(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return CreatedAtAction(nameof(ObtenerPorId), new { id = curso!.IdCurso }, curso);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] CursoUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje) = await _cursoService.Actualizar(id, IdEscuelaActual, dto);
        if (!exito) return NotFound(new { mensaje });

        return Ok(new { mensaje });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var (exito, mensaje) = await _cursoService.Eliminar(id, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return Ok(new { mensaje });
    }
}
