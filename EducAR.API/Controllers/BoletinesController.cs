using EducAR.API.DTOs.Boletines;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BoletinesController : ControllerBase
{
    private readonly IBoletinService _boletinService;

    public BoletinesController(IBoletinService boletinService)
    {
        _boletinService = boletinService;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    // GET api/boletines/curso/1/periodo/1
    [HttpGet("curso/{idCurso}/periodo/{idPeriodo}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorCursoYPeriodo(int idCurso, int idPeriodo)
    {
        var boletines = await _boletinService.ObtenerPorCursoYPeriodo(idCurso, idPeriodo, IdEscuelaActual);
        return Ok(boletines);
    }

    // GET api/boletines/alumno/1/curso/1/periodo/1
    [HttpGet("alumno/{idAlumno}/curso/{idCurso}/periodo/{idPeriodo}")]
    [Authorize(Roles = "Administrador,Docente,Tutor")]
    public async Task<IActionResult> ObtenerPorAlumno(int idAlumno, int idCurso, int idPeriodo)
    {
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

        var (exito, mensaje, generados) = await _boletinService.Generar(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return Ok(new { mensaje, boletinesGenerados = generados });
    }

    // PATCH api/boletines/5/observacion
    [HttpPatch("{id}/observacion")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ActualizarObservacion(int id, [FromBody] BoletinObservacionDto dto)
    {
        var (exito, mensaje) = await _boletinService.ActualizarObservacion(id, dto, IdEscuelaActual);
        if (!exito) return NotFound(new { mensaje });

        return Ok(new { mensaje });
    }
}