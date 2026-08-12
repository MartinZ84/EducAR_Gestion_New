using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class DocenteMateriaCurso
{
    [Key]
    public int IdDocenteMateriaCurso { get; set; }
    public int IdDocente { get; set; }
    public int IdMateria { get; set; }
    public int IdCurso { get; set; }
    public DateTime FechaAsignacion { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public Docente Docente { get; set; } = null!;
    public Materia Materia { get; set; } = null!;
    public Curso Curso { get; set; } = null!;
}