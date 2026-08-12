using EducAR.API.DTOs.Mensajes;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MensajesController : ControllerBase
{
    private readonly IMensajeService _mensajeService;

    public MensajesController(IMensajeService mensajeService)
    {
        _mensajeService = mensajeService;
    }

    private int IdUsuarioActual =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    // GET api/mensajes/recibidos
    // [HttpGet("recibidos")]
    // public async Task<IActionResult> ObtenerRecibidos()
    // {
    //     var mensajes = await _mensajeService.ObtenerRecibidos(IdUsuarioActual);
    //     return Ok(mensajes);
    // }

    // // GET api/mensajes/enviados
    // [HttpGet("enviados")]
    // public async Task<IActionResult> ObtenerEnviados()
    // {
    //     var mensajes = await _mensajeService.ObtenerEnviados(IdUsuarioActual);
    //     return Ok(mensajes);
    // }

    [HttpGet("recibidos")]
    public async Task<IActionResult> ObtenerRecibidos(
    [FromQuery] int pagina = 1, [FromQuery] int cantidad = 10)
    {
        var resultado = await _mensajeService.ObtenerRecibidosPaginado(IdUsuarioActual, pagina, cantidad);
        return Ok(resultado);
    }

    [HttpGet("enviados")]
    public async Task<IActionResult> ObtenerEnviados(
        [FromQuery] int pagina = 1, [FromQuery] int cantidad = 10)
    {
        var resultado = await _mensajeService.ObtenerEnviadosPaginado(IdUsuarioActual, pagina, cantidad);
        return Ok(resultado);
    }

    // GET api/mensajes/noleidos
    [HttpGet("noleidos")]
    public async Task<IActionResult> ContarNoLeidos()
    {
        var cantidad = await _mensajeService.ContarNoLeidos(IdUsuarioActual);
        return Ok(new { noLeidos = cantidad });
    }

    // GET api/mensajes/5
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var mensaje = await _mensajeService.ObtenerPorId(id, IdUsuarioActual);
        if (mensaje is null) return NotFound(new { mensaje = "Mensaje no encontrado." });
        return Ok(mensaje);
    }

    // POST api/mensajes
    [HttpPost]
    [Authorize(Roles = "Administrador,Docente,Tutor")]
    public async Task<IActionResult> Enviar([FromBody] MensajeCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje) = await _mensajeService.Enviar(dto, IdUsuarioActual, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return Ok(new { mensaje });
    }

    // PATCH api/mensajes/5/leido
    [HttpPatch("{id}/leido")]
    public async Task<IActionResult> MarcarLeido(int id)
    {
        var exito = await _mensajeService.MarcarLeido(id, IdUsuarioActual);
        if (!exito) return NotFound(new { mensaje = "Mensaje no encontrado." });

        return Ok(new { mensaje = "Mensaje marcado como leído." });
    }

    // DELETE api/mensajes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var exito = await _mensajeService.Eliminar(id, IdUsuarioActual);
        if (!exito) return NotFound(new { mensaje = "Mensaje no encontrado." });

        return Ok(new { mensaje = "Mensaje eliminado correctamente." });
    }
}