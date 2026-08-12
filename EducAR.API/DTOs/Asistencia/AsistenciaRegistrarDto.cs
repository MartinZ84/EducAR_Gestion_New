using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.Asistencia;

public class AsistenciaRegistrarDto
{
    [Required(ErrorMessage = "El curso es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un curso válido.")]
    public int IdCurso { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "Debe incluir al menos un alumno.")]
    [MinLength(1, ErrorMessage = "Debe incluir al menos un alumno.")]
    public List<AsistenciaAlumnoDto> Alumnos { get; set; } = new();
}

public class AsistenciaAlumnoDto
{
    [Required(ErrorMessage = "El alumno es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un alumno válido.")]
    public int IdAlumno { get; set; }

    public bool Presente { get; set; }
}
