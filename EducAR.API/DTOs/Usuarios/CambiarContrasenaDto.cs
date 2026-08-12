using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.Usuarios;

public class CambiarContrasenaDto
{
    [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
    public string ContrasenaActual { get; set; } = null!;

    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres.")]
    public string NuevaContrasena { get; set; } = null!;

    [Required(ErrorMessage = "La confirmación de contraseña es obligatoria.")]
    [Compare("NuevaContrasena", ErrorMessage = "La confirmación no coincide con la nueva contraseña.")]
    public string ConfirmarContrasena { get; set; } = null!;
}
