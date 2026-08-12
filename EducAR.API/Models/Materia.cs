using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class Materia
{
    [Key]
    public int IdMateria { get; set; }
    public int IdEscuela { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public Escuela Escuela { get; set; } = null!;
    public ICollection<DocenteMateriaCurso> DocenteMateriaCursos { get; set; } = new List<DocenteMateriaCurso>();
    public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
    public ICollection<DetalleBoletin> DetallesBoletines { get; set; } = new List<DetalleBoletin>();
}