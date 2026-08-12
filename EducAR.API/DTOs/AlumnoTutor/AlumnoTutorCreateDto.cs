using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.AlumnoTutor;

public class AlumnoTutorCreateDto
{
    [Required(ErrorMessage = "El alumno es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un alumno válido.")]
    public int IdAlumno { get; set; }

    [Required(ErrorMessage = "El tutor es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tutor válido.")]
    public int IdTutor { get; set; }

    [MaxLength(80, ErrorMessage = "El parentesco no puede superar los 80 caracteres.")]
    public string? Parentesco { get; set; }

    public bool EsResponsablePrinc { get; set; }
}
