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
    public DateTime? FechaNacimiento { get; set; }
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
    public ICollection<TelefonoContacto> Telefonos { get; set; } = new List<TelefonoContacto>();
    public string? Calle { get; set; }
    public string? Numero { get; set; }
    public string? Piso { get; set; }
    public string? Departamento { get; set; }
    public string? Barrio { get; set; }
    public string? Localidad { get; set; }
    public string? Provincia { get; set; }
}