using EducAR.API.DTOs.DocenteMateriaCurso;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocenteMateriaCursoController : ControllerBase
{
    private readonly IDocenteMateriaCursoService _docenteMateriaCursoService;

    public DocenteMateriaCursoController(IDocenteMateriaCursoService docenteMateriaCursoService)
    {
        _docenteMateriaCursoService = docenteMateriaCursoService;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    // GET api/docentemateriacurso/docente/1
    [HttpGet("docente/{idDocente}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorDocente(int idDocente)
    {
        var asignaciones = await _docenteMateriaCursoService.ObtenerPorDocente(idDocente, IdEscuelaActual);
        return Ok(asignaciones);
    }

    // GET api/docentemateriacurso/curso/1
    [HttpGet("curso/{idCurso}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorCurso(int idCurso)
    {
        var asignaciones = await _docenteMateriaCursoService.ObtenerPorCurso(idCurso, IdEscuelaActual);
        return Ok(asignaciones);
    }

    // GET api/docentemateriacurso/curso/1/materia/2
    [HttpGet("curso/{idCurso}/materia/{idMateria}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorCursoYMateria(int idCurso, int idMateria)
    {
        var asignaciones = await _docenteMateriaCursoService
            .ObtenerPorCursoYMateria(idCurso, idMateria, IdEscuelaActual);
        return Ok(asignaciones);
    }

    // POST api/docentemateriacurso
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Asignar([FromBody] DocenteMateriaCursoCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje, asignacion) = await _docenteMateriaCursoService.Asignar(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return Ok(asignacion);
    }

    // DELETE api/docentemateriacurso/1
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Desasignar(int id)
    {
        var (exito, mensaje) = await _docenteMateriaCursoService.Desasignar(id, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return Ok(new { mensaje });
    }

    // GET api/docentemateriacurso/mis-cursos
    [HttpGet("mis-cursos")]
    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> ObtenerMisCursos()
    {
        var idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var cursos = await _docenteMateriaCursoService.ObtenerMisCursos(idUsuario, IdEscuelaActual);
        return Ok(cursos);
    }
}