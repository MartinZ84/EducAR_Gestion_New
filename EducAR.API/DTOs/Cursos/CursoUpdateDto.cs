using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.Cursos;

public class CursoUpdateDto
{
    [Required(ErrorMessage = "El grado es obligatorio.")]
    [Range(1, 7, ErrorMessage = "El grado debe estar entre 1 y 7.")]
    public int Grado { get; set; }

    [Required(ErrorMessage = "La división es obligatoria.")]
    [MaxLength(10, ErrorMessage = "La división no puede superar los 10 caracteres.")]
    public string Division { get; set; } = null!;

    [MaxLength(50, ErrorMessage = "El turno no puede superar los 50 caracteres.")]
    public string? Turno { get; set; }

    public bool Activo { get; set; }
}
