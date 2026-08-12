using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.Materias;

public class MateriaCreateDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Nombre { get; set; } = null!;

    [MaxLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
    public string? Descripcion { get; set; }
}
