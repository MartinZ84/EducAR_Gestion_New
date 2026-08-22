namespace EducAR.API.DTOs.CiclosLectivos;

public class CicloLectivoDetalleDto
{
    public int IdCicloLectivo { get; set; }
    public int Anio { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activo { get; set; }
    public List<CursoCicloLectivoDetalleDto> Cursos { get; set; } = new();
    public int CantidadCursos { get; set; }
    public int CantidadAlumnosMatriculados { get; set; }
}

public class CursoCicloLectivoDetalleDto { public int IdCurso { get; set; } public string Curso { get; set; } = null!; public int CantidadAlumnos { get; set; } }
