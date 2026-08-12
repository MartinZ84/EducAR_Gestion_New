using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class Escuela
{
    [Key]
    public int IdEscuela { get; set; }
    public string Nombre { get; set; } = null!;
    public string Direccion { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
    public ICollection<CicloLectivo> CiclosLectivos { get; set; } = new List<CicloLectivo>();
    public ICollection<Materia> Materias { get; set; } = new List<Materia>();
    public ICollection<Alumno> Alumnos { get; set; } = new List<Alumno>();
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}