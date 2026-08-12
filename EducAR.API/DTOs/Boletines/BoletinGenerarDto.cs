using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.Boletines;

public class BoletinGenerarDto
{
    [Required(ErrorMessage = "El curso es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un curso válido.")]
    public int IdCurso { get; set; }

    [Required(ErrorMessage = "El período de evaluación es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un período válido.")]
    public int IdPeriodoEvaluacion { get; set; }
}
