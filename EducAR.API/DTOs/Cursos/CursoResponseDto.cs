namespace EducAR.API.DTOs.Cursos;

public class CursoResponseDto
{
    public int IdCurso { get; set; }
    public int IdEscuela { get; set; }
    public int IdCicloLectivo { get; set; }
    public int Anio { get; set; }
    public int Grado { get; set; }
    public string Division { get; set; } = null!;
    public string? Turno { get; set; }
    public bool Activo { get; set; }
    public int CantidadAlumnos { get; set; }
}