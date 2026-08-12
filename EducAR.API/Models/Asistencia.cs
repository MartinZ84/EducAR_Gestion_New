using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class Asistencia
{
    [Key]
    public int IdAsistencia { get; set; }
    public int IdDocente { get; set; }
    public int IdAlumno { get; set; }
    public int IdCurso { get; set; }
    public DateTime Fecha { get; set; }
    public bool Presente { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public Docente Docente { get; set; } = null!;
    public Alumno Alumno { get; set; } = null!;
    public Curso Curso { get; set; } = null!;
}