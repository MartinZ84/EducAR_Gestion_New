using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class DetalleBoletin
{
    [Key]
    public int IdDetalleBoletin { get; set; }
    public int IdBoletin { get; set; }
    public int IdMateria { get; set; }
    public decimal CalificacionFinal { get; set; }
    public string? ConceptoFinal { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public Boletin Boletin { get; set; } = null!;
    public Materia Materia { get; set; } = null!;
}