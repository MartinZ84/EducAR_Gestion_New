namespace EducAR.API.DTOs.Usuarios;

public class UsuarioResponseDto
{
    public int IdUsuario { get; set; }
    public int IdRol { get; set; }
    public string Rol { get; set; } = null!;
    public int IdEscuela { get; set; }
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string NombreUsuario { get; set; } = null!;
    public bool Activo { get; set; }
    public DateTime? UltimoAcceso { get; set; }
}