using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class NotaEvaluacion
{
    [Key]
    public int IdNotaEvaluacion { get; set; }
    public int IdEvaluacion { get; set; }
    public int IdAlumno { get; set; }
    public decimal Valor { get; set; }
    public DateTime FechaAct { get; set; } = DateTime.Now;

    public Evaluacion Evaluacion { get; set; } = null!;
    public Alumno Alumno { get; set; } = null!;
}
