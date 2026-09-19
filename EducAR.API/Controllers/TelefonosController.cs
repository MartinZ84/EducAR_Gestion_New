using EducAR.API.DTOs.Telefonos;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TelefonosController : ControllerBase
{
    private readonly ITelefonoService _telefonoService;

    public TelefonosController(ITelefonoService telefonoService)
    {
        _telefonoService = telefonoService;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    // GET api/telefonos/alumno/5
    [HttpGet("alumno/{idAlumno}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorAlumno(int idAlumno)
    {
        var telefonos = await _telefonoService.ObtenerPorAlumnoAsync(idAlumno, IdEscuelaActual);
        return Ok(telefonos);
    }

    // POST api/telefonos
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] TelefonoCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje, telefono) = await _telefonoService.CrearAsync(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return Ok(telefono);
    }

    // DELETE api/telefonos/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var (exito, mensaje) = await _telefonoService.EliminarAsync(id, IdEscuelaActual);
        if (!exito) return NotFound(new { mensaje });

        return Ok(new { mensaje });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] TelefonoCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var (exito, mensaje, telefono) = await _telefonoService.ActualizarAsync(id, dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });
        return Ok(telefono);
    }
}
