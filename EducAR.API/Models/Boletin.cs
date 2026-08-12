using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class Boletin
{
      public Boletin()
    {
        Detalles = new HashSet<DetalleBoletin>();
    }
    [Key]
    public int IdBoletin { get; set; }
    public int IdAlumno { get; set; }
    public int IdCurso { get; set; }
    public int IdPeriodoEvaluacion { get; set; }
    public string? ObservacionGeneral { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public Alumno Alumno { get; set; } = null!;
    public Curso Curso { get; set; } = null!;
    public PeriodoEvaluacion PeriodoEvaluacion { get; set; } = null!;
    public ICollection<DetalleBoletin> DetallesBoletines { get; set; } = new List<DetalleBoletin>();

     // Propiedad de navegación - colección de detalles
    public virtual ICollection<DetalleBoletin> Detalles { get; set; }
}