namespace EducAR.API.DTOs.DocenteMateriaCurso;

public class DocenteMateriaCursoResponseDto
{
    public int IdDocenteMateriaCurso { get; set; }
    public int IdDocente { get; set; }
    public string NombreDocente { get; set; } = null!;
    public string ApellidoDocente { get; set; } = null!;
    public int IdMateria { get; set; }
    public string NombreMateria { get; set; } = null!;
    public int IdCurso { get; set; }
    public string Curso { get; set; } = null!;
    public int Grado { get; set; }
    public string Division { get; set; } = null!;
    public string? Turno { get; set; }
    public int Anio { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public bool Activo { get; set; }
}
