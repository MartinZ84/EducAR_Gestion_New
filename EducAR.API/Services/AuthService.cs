using EducAR.API.DTOs.Auth;
using EducAR.API.Helpers;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;

namespace EducAR.API.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly JwtHelper _jwtHelper;

    public AuthService(IAuthRepository authRepository, JwtHelper jwtHelper)
    {
        _authRepository = authRepository;
        _jwtHelper = jwtHelper;
    }

    public async Task<LoginResponseDto?> Login(LoginRequestDto request)
    {
        // 1. Buscar usuario
        var usuario = await _authRepository.ObtenerPorUsuarioYEscuela(
            request.NombreUsuario,
            request.IdEscuela
        );

        if (usuario is null) return null;

        // 2. Verificar contraseña con BCrypt
        bool contrasenaValida = BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.HashContrasena);
        if (!contrasenaValida) return null;

        // 3. Generar token
        var token = _jwtHelper.GenerarToken(usuario);

        // 4. Actualizar último acceso
        await _authRepository.ActualizarUltimoAcceso(usuario.IdUsuario);

        return new LoginResponseDto
        {
            Token         = token,
            NombreUsuario = usuario.NombreUsuario,
            NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}",
            Rol           = usuario.Rol.Nombre,
            IdEscuela     = usuario.IdEscuela,
            Expiracion    = DateTime.UtcNow.AddHours(8)
        };
    }
}