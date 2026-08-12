using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class Matricula
{
    [Key]
    public int IdMatricula { get; set; }

    public int IdEscuela { get; set; }
    public int IdAlumno { get; set; }
    public int IdCurso { get; set; }
    public int IdCicloLectivo { get; set; }

    public DateTime FechaMatricula { get; set; } = DateTime.Now;
    public DateTime? FechaBaja { get; set; }
    public EstadoMatricula Estado { get; set; } = EstadoMatricula.Activa;

    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    public Escuela Escuela { get; set; } = null!;
    public Alumno Alumno { get; set; } = null!;
    public Curso Curso { get; set; } = null!;
    public CicloLectivo CicloLectivo { get; set; } = null!;
}
