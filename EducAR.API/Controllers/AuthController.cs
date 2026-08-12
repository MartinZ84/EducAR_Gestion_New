using EducAR.API.DTOs.Auth;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var resultado = await _authService.Login(request);

        if (resultado is null)
            return Unauthorized(new { mensaje = "Usuario, contraseña o escuela incorrectos." });

        return Ok(resultado);
    }
}