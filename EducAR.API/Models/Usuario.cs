using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class Usuario
{
    [Key]
    public int IdUsuario { get; set; }
    public int IdRol { get; set; }
    public int IdEscuela { get; set; }
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string NombreUsuario { get; set; } = null!;
    public string HashContrasena { get; set; } = null!;
    public bool Activo { get; set; } = true;
    public DateTime? UltimoAcceso { get; set; }
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public Rol Rol { get; set; } = null!;
    public Escuela Escuela { get; set; } = null!;
    public Docente? Docente { get; set; }
    public Tutor? Tutor { get; set; }
    public ICollection<Mensaje> MensajesEnviados { get; set; } = new List<Mensaje>();
    public ICollection<Mensaje> MensajesRecibidos { get; set; } = new List<Mensaje>();
}