using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class Docente
{
    [Key]
    public int IdDocente { get; set; }
    public int IdUsuario { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public Usuario Usuario { get; set; } = null!;
    public ICollection<DocenteMateriaCurso> DocenteMateriaCursos { get; set; } = new List<DocenteMateriaCurso>();
    public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
}