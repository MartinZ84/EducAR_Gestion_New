using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre de usuario no puede superar los 100 caracteres.")]
    public string NombreUsuario { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public string Contrasena { get; set; } = null!;

    [Required(ErrorMessage = "La escuela es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una escuela válida.")]
    public int IdEscuela { get; set; }
}
