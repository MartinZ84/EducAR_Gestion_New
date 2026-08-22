namespace EducAR.API.DTOs.Usuarios;

public class UsuarioDetalleDto
{
    public int IdUsuario { get; set; }
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Rol { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool Activo { get; set; }
    public DateTime FechaCrea { get; set; }
    public DateTime FechaAct { get; set; }
}
