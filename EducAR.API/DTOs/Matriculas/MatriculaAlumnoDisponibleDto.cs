namespace EducAR.API.DTOs.Matriculas;

public class MatriculaAlumnoDisponibleDto
{
    public int IdAlumno { get; set; }
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public DateTime? FecNac { get; set; }
    public bool Matriculado { get; set; }
    public int? IdMatricula { get; set; }
    public int? IdCursoActual { get; set; }
    public string? CursoActual { get; set; }
    public string? EstadoMatricula { get; set; }
}
