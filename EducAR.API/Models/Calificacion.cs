using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class Calificacion
{
    [Key]
    public int IdCalificacion { get; set; }
    public int IdAlumno { get; set; }
    public int IdMateria { get; set; }
    public int IdPeriodoEvaluacion { get; set; }
    public decimal ValorCalificacion { get; set; }
    public int? NivelCalificacion { get; set; }
    public string? Observacion { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public Alumno Alumno { get; set; } = null!;
    public Materia Materia { get; set; } = null!;
    public PeriodoEvaluacion PeriodoEvaluacion { get; set; } = null!;
}