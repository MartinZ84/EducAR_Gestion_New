namespace EducAR.API.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = null!;
    public string NombreUsuario { get; set; } = null!;
    public string NombreCompleto { get; set; } = null!;
    public string Rol { get; set; } = null!;
    public int IdEscuela { get; set; }
    public DateTime Expiracion { get; set; }
}