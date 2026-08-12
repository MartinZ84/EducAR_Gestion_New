using EducAR.API.DTOs.Usuarios;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // requiere JWT válido
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    // Helper para sacar el IdEscuela del token
    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    // [HttpGet]
    // [Authorize(Roles = "Administrador")]
    // public async Task<IActionResult> ObtenerTodos()
    // {
    //     var usuarios = await _usuarioService.ObtenerTodos(IdEscuelaActual);
    //     return Ok(usuarios);
    // }
    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerTodos([FromQuery] int pagina = 1, [FromQuery] int cantidad = 10)
    {
        var resultado = await _usuarioService.ObtenerTodosPaginado(IdEscuelaActual, pagina, cantidad);
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var usuario = await _usuarioService.ObtenerPorId(id, IdEscuelaActual);
        if (usuario is null) return NotFound(new { mensaje = "Usuario no encontrado." });
        return Ok(usuario);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] UsuarioCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Forzamos que el usuario se cree en la escuela del admin logueado
        dto.IdEscuela = IdEscuelaActual;

        var (exito, mensaje, usuario) = await _usuarioService.Crear(dto);
        if (!exito) return BadRequest(new { mensaje });

        return CreatedAtAction(nameof(ObtenerPorId), new { id = usuario!.IdUsuario }, usuario);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] UsuarioUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje) = await _usuarioService.Actualizar(id, IdEscuelaActual, dto);
        if (!exito) return NotFound(new { mensaje });

        return Ok(new { mensaje });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var exito = await _usuarioService.Eliminar(id, IdEscuelaActual);
        if (!exito) return NotFound(new { mensaje = "Usuario no encontrado." });

        return Ok(new { mensaje = "Usuario dado de baja correctamente." });
    }

    // PATCH api/usuarios/cambiar-contrasena
    [HttpPatch("cambiar-contrasena")]
    public async Task<IActionResult> CambiarContrasena([FromBody] CambiarContrasenaDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (exito, mensaje) = await _usuarioService.CambiarContrasena(idUsuario, dto);
        if (!exito) return BadRequest(new { mensaje });

        return Ok(new { mensaje });
    }

    // GET api/usuarios/perfil
    [HttpGet("perfil")]
    public async Task<IActionResult> ObtenerPerfil()
    {
        var idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var perfil = await _usuarioService.ObtenerPerfil(idUsuario);
        if (perfil is null) return NotFound(new { mensaje = "Usuario no encontrado." });
        return Ok(perfil);
    }
}