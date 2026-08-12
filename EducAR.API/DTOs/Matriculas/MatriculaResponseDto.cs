namespace EducAR.API.DTOs.Matriculas;

public class MatriculaResponseDto
{
    public int IdMatricula { get; set; }
    public int IdEscuela { get; set; }
    public int IdAlumno { get; set; }
    public int Dni { get; set; }
    public string NombreAlumno { get; set; } = null!;
    public string ApellidoAlumno { get; set; } = null!;
    public DateOnly FecNac { get; set; }
    public int IdCurso { get; set; }
    public string Curso { get; set; } = null!;
    public int IdCicloLectivo { get; set; }
    public int Anio { get; set; }
    public DateTime FechaMatricula { get; set; }
    public DateTime? FechaBaja { get; set; }
    public string Estado { get; set; } = null!;
}
