using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class Alumno
{
    [Key]
    public int IdAlumno { get; set; }
    public int IdEscuela { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public int Dni { get; set; }
    public DateOnly FecNac { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public Escuela Escuela { get; set; } = null!;
    public ICollection<AlumnoTutor> AlumnoTutores { get; set; } = new List<AlumnoTutor>();
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
    public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
    public ICollection<Boletin> Boletines { get; set; } = new List<Boletin>();
}