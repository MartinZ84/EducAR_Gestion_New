using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.Telefonos;

public class TelefonoCreateDto
{
    [Required(ErrorMessage = "El id del alumno es obligatorio.")]
    public int IdAlumno { get; set; }

    [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
    [MaxLength(50, ErrorMessage = "Máximo 50 caracteres.")]
    public string Numero { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Tipo { get; set; }

    public bool EsPrincipal { get; set; } = false;
}