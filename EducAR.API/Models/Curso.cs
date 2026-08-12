using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class Curso
{
    [Key]
    public int IdCurso { get; set; }
    public int IdEscuela { get; set; }
    public int IdCicloLectivo { get; set; }
    public int Grado { get; set; }
    public string Division { get; set; } = null!;
    public string? Turno { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public Escuela Escuela { get; set; } = null!;
    public CicloLectivo CicloLectivo { get; set; } = null!;
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    public ICollection<DocenteMateriaCurso> DocenteMateriaCursos { get; set; } = new List<DocenteMateriaCurso>();
    public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
    public ICollection<Boletin> Boletines { get; set; } = new List<Boletin>();
}