using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class Evaluacion
{
    [Key]
    public int IdEvaluacion { get; set; }
    public int IdCurso { get; set; }
    public int IdMateria { get; set; }
    public int IdPeriodoEvaluacion { get; set; }
    [MaxLength(200)]
    public string Titulo { get; set; } = null!;
    [MaxLength(4000)]
    public string Temario { get; set; } = "";
    [MaxLength(4000)]
    public string Descripcion { get; set; } = "";
    public DateTime Fecha { get; set; }
    public bool Activo { get; set; } = true;

    public Curso Curso { get; set; } = null!;
    public Materia Materia { get; set; } = null!;
    public PeriodoEvaluacion PeriodoEvaluacion { get; set; } = null!;
    public ICollection<NotaEvaluacion> Notas { get; set; } = new List<NotaEvaluacion>();
}
