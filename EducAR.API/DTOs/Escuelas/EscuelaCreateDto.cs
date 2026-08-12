using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.Escuelas;

public class EscuelaCreateDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(200, ErrorMessage = "El nombre no puede superar los 200 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [MaxLength(300, ErrorMessage = "La dirección no puede superar los 300 caracteres.")]
    public string Direccion { get; set; } = null!;

    [MaxLength(50, ErrorMessage = "El teléfono no puede superar los 50 caracteres.")]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    [MaxLength(150, ErrorMessage = "El email no puede superar los 150 caracteres.")]
    public string? Email { get; set; }
}
