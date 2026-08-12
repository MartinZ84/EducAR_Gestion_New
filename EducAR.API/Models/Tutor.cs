using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class Tutor
{
    [Key]
    public int IdTutor { get; set; }
    public int IdUsuario { get; set; }
    public bool EsResponsable { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public Usuario Usuario { get; set; } = null!;
    public ICollection<AlumnoTutor> AlumnoTutores { get; set; } = new List<AlumnoTutor>();
}