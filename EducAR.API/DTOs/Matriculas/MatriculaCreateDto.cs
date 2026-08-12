using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.Matriculas;

public class MatriculaCreateDto
{
    [Required]
    public int IdAlumno { get; set; }

    [Required]
    public int IdCurso { get; set; }
}
