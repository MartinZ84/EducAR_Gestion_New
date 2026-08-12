namespace EducAR.API.DTOs.Usuarios;

public class PerfilResponseDto
{
    public int IdUsuario { get; set; }
    public string NombreCompleto { get; set; } = null!;
    public string NombreUsuario { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Rol { get; set; } = null!;
    public string NombreEscuela { get; set; } = null!;
    public DateTime? UltimoAcceso { get; set; }
}