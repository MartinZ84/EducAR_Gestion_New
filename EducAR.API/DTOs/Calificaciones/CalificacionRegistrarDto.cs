using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.Calificaciones;

public class CalificacionRegistrarDto
{
    [Required(ErrorMessage = "El curso es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un curso válido.")]
    public int IdCurso { get; set; }

    [Required(ErrorMessage = "La materia es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una materia válida.")]
    public int IdMateria { get; set; }

    [Required(ErrorMessage = "El período de evaluación es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un período válido.")]
    public int IdPeriodoEvaluacion { get; set; }

    [Required(ErrorMessage = "Debe incluir al menos un alumno.")]
    [MinLength(1, ErrorMessage = "Debe incluir al menos un alumno.")]
    public List<CalificacionAlumnoDto> Alumnos { get; set; } = new();
}

public class CalificacionAlumnoDto
{
    [Required(ErrorMessage = "El alumno es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un alumno válido.")]
    public int IdAlumno { get; set; }

    [Required(ErrorMessage = "La calificación es obligatoria.")]
    [Range(1, 10, ErrorMessage = "La calificación debe estar entre 1 y 10.")]
    public decimal ValorCalificacion { get; set; }

    [MaxLength(500, ErrorMessage = "La observación no puede superar los 500 caracteres.")]
    public string? Observacion { get; set; }
}
