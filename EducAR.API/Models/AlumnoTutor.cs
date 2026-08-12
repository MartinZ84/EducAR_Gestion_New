using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class AlumnoTutor
{
    [Key]
    public int IdAlumnoTutor { get; set; }
    public int IdAlumno { get; set; }
    public int IdTutor { get; set; }
    public string? Parentesco { get; set; }
    public bool EsResponsablePrinc { get; set; } = false;
    public bool Activo { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public Alumno Alumno { get; set; } = null!;
    public Tutor Tutor { get; set; } = null!;
}