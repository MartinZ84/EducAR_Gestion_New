namespace EducAR.API.DTOs.Alumnos;

public class AlumnoMatriculaDto
{
    public int IdMatricula { get; set; }
    public int IdCurso { get; set; }
    public int Grado { get; set; }
    public string Division { get; set; } = null!;
    public string? Turno { get; set; }
    public int IdCicloLectivo { get; set; }
    public int Anio { get; set; }
    public string Estado { get; set; } = null!;
    public DateTime FechaMatricula { get; set; }
    public DateTime? FechaBaja { get; set; }
}
