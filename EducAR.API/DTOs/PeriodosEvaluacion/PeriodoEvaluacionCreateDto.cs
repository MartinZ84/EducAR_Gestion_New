using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.PeriodosEvaluacion;

public class PeriodoEvaluacionCreateDto
{
    [Required(ErrorMessage = "El ciclo lectivo es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un ciclo lectivo válido.")]
    public int IdCicloLectivo { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    public DateTime FechaFin { get; set; }
}
