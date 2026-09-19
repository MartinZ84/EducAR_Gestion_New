using EducAR.API.DTOs.Matriculas;
using EducAR.API.Data;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatriculasController : ControllerBase
{
    private readonly IMatriculaService _service;
    private readonly AppDbContext? _context;

    public MatriculasController(IMatriculaService service, AppDbContext? context = null)
    {
        _service = service;
        _context = context;
    }

    private int IdEscuelaActual => int.Parse(User.FindFirstValue("IdEscuela")!);
    private int IdUsuarioActual => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("curso/{idCurso}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorCurso(int idCurso)
    {
        if (User.IsInRole("Docente") && (_context is null ||
            !await _context.DocenteMateriaCursos.AnyAsync(a => a.IdCurso == idCurso && a.Activo &&
                a.Docente.IdUsuario == IdUsuarioActual && a.Curso.IdEscuela == IdEscuelaActual)))
            return Forbid();
        return Ok(await _service.ObtenerPorCurso(idCurso, IdEscuelaActual));
    }

    [HttpGet("alumno/{idAlumno}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorAlumno(int idAlumno)
    {
        return Ok(await _service.ObtenerPorAlumno(idAlumno, IdEscuelaActual));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var matricula = await _service.ObtenerPorId(id, IdEscuelaActual);
        return matricula is null ? NotFound(new { mensaje = "Matrícula no encontrada." }) : Ok(matricula);
    }

    [HttpGet("alumnos-disponibles")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerAlumnosDisponibles(
        [FromQuery] int anioRegistro,
        [FromQuery] int idCicloLectivo,
        [FromQuery] PaginacionDto paginacion,
        [FromQuery] string? nombre,
        [FromQuery] string? apellido,
        [FromQuery] int? dni)
    {
        if (anioRegistro < 2000 || anioRegistro > 2100)
            return BadRequest(new { mensaje = "El año de registro no es válido." });

        var resultado = await _service.ObtenerAlumnosParaMatricular(
            IdEscuelaActual, anioRegistro, idCicloLectivo, paginacion, nombre, apellido, dni);
        return Ok(resultado);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] MatriculaCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje, matricula) = await _service.Crear(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return CreatedAtAction(nameof(ObtenerPorId), new { id = matricula!.IdMatricula }, matricula);
    }

    [HttpPost("asignar-masivo")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> AsignarMasivo([FromBody] MatriculaAsignacionMasivaDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje, resultado) = await _service.AsignarMasivo(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje, resultado });

        return Ok(new { mensaje, resultado });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DarDeBaja(int id)
    {
        var (exito, mensaje) = await _service.DarDeBaja(id, IdEscuelaActual);
        return exito ? Ok(new { mensaje }) : NotFound(new { mensaje });
    }
}
