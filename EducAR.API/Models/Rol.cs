using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class Rol
{
    [Key]
    public int IdRol { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Activo { get; set; } = true;

    // Navegación
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}