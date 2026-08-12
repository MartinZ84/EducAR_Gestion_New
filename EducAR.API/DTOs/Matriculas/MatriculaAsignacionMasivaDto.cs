using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.Matriculas;

public class MatriculaAsignacionMasivaDto
{
    [Required]
    public int IdCurso { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Debe seleccionar al menos un alumno.")]
    public List<int> IdsAlumnos { get; set; } = new();
}
