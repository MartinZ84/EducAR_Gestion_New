using EducAR.API.DTOs.Boletines;
using EducAR.API.Data;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BoletinesController : ControllerBase
{
    private readonly IBoletinService _boletinService;
    private readonly AppDbContext _context;

    public BoletinesController(IBoletinService boletinService, AppDbContext context)
    {
        _boletinService = boletinService;
        _context = context;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);
    private int IdUsuarioActual => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private Task<bool> PuedeVerCurso(int idCurso) => User.IsInRole("Administrador")
        ? Task.FromResult(true)
        : _context.DocenteMateriaCursos.AnyAsync(a => a.IdCurso == idCurso && a.Activo &&
            a.Docente.IdUsuario == IdUsuarioActual && a.Curso.IdEscuela == IdEscuelaActual);

    // GET api/boletines/curso/1/periodo/1
    [HttpGet("curso/{idCurso}/periodo/{idPeriodo}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorCursoYPeriodo(int idCurso, int idPeriodo)
    {
        if (!await PuedeVerCurso(idCurso)) return Forbid();
        var boletines = await _boletinService.ObtenerPorCursoYPeriodo(idCurso, idPeriodo, IdEscuelaActual);
        return Ok(boletines);
    }

    // GET api/boletines/alumno/1/curso/1/periodo/1
    [HttpGet("alumno/{idAlumno}/curso/{idCurso}/periodo/{idPeriodo}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorAlumno(int idAlumno, int idCurso, int idPeriodo)
    {
        if (!await PuedeVerCurso(idCurso)) return Forbid();
        var boletin = await _boletinService.ObtenerPorAlumno(idAlumno, idCurso, idPeriodo, IdEscuelaActual);
        if (boletin is null) return NotFound(new { mensaje = "Boletín no encontrado." });
        return Ok(boletin);
    }

    // POST api/boletines/generar
    [HttpPost("generar")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> Generar([FromBody] BoletinGenerarDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await PuedeVerCurso(dto.IdCurso)) return Forbid();

        var (exito, mensaje, generados) = await _boletinService.Generar(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return Ok(new { mensaje, boletinesGenerados = generados });
    }

    [HttpPost("alumno/{idAlumno}/curso/{idCurso}/periodo/{idPeriodo}/generar")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> GenerarParaAlumno(int idAlumno, int idCurso, int idPeriodo)
    {
        if (!await PuedeVerCurso(idCurso)) return Forbid();
        var (exito, mensaje, boletin) = await _boletinService.GenerarParaAlumno(
            idAlumno, idCurso, idPeriodo, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });
        return Ok(boletin);
    }

    // PATCH api/boletines/5/observacion
    [HttpPatch("{id}/observacion")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ActualizarObservacion(int id, [FromBody] BoletinObservacionDto dto)
    {
        if (User.IsInRole("Docente"))
        {
            var idCurso = await _context.Boletines.Where(b => b.IdBoletin == id)
                .Select(b => (int?)b.IdCurso).FirstOrDefaultAsync();
            if (idCurso is null || !await PuedeVerCurso(idCurso.Value)) return Forbid();
        }
        var (exito, mensaje) = await _boletinService.ActualizarObservacion(id, dto, IdEscuelaActual);
        if (!exito) return NotFound(new { mensaje });

        return Ok(new { mensaje });
    }
}
