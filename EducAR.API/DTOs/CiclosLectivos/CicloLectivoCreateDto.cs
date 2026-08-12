using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.CiclosLectivos;

public class CicloLectivoCreateDto
{
    [Required(ErrorMessage = "El año es obligatorio.")]
    [Range(2000, 2100, ErrorMessage = "El año debe estar entre 2000 y 2100.")]
    public int Anio { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    public DateTime FechaFin { get; set; }
}
