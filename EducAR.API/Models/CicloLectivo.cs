using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class CicloLectivo
{
    [Key]
    public int IdCicloLectivo { get; set; }
    public int IdEscuela { get; set; }
    public int Anio { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public Escuela Escuela { get; set; } = null!;
    public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    public ICollection<PeriodoEvaluacion> PeriodosEvaluacion { get; set; } = new List<PeriodoEvaluacion>();
}