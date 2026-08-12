using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class PeriodoEvaluacion
{
    [Key]
    public int IdPeriodoEvaluacion { get; set; }
    public int IdCicloLectivo { get; set; }
    public string Nombre { get; set; } = null!;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public CicloLectivo CicloLectivo { get; set; } = null!;
    public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
    public ICollection<Boletin> Boletines { get; set; } = new List<Boletin>();
}