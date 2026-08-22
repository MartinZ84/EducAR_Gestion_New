namespace EducAR.API.DTOs.Cursos;

public class CursoDetalleDto
{
    public int IdCurso { get; set; }
    public string Nombre { get; set; } = null!;
    public string Division { get; set; } = null!;
    public string? Turno { get; set; }
    public string CicloLectivo { get; set; } = null!;
    public List<AlumnoCursoDetalleDto> AlumnosMatriculados { get; set; } = new();
    public int CantidadTotalAlumnos { get; set; }
}

public class AlumnoCursoDetalleDto { public int IdAlumno { get; set; } public int Dni { get; set; } public string NombreCompleto { get; set; } = null!; }
