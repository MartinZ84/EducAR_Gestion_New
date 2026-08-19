using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducAR.API.Models;

[Table("TelefonosContacto")]
public class TelefonoContacto
{
    [Key]
    public int IdTelefono { get; set; }

    [Required]
    public int IdAlumno { get; set; }

    [Required]
    [MaxLength(50)]
    public string Numero { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Tipo { get; set; }

    public bool EsPrincipal { get; set; } = false;

    // Navigation
    public Alumno Alumno { get; set; } = null!;
}