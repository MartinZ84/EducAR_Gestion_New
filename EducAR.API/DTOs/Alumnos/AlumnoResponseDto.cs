namespace EducAR.API.DTOs.Alumnos;

public class AlumnoResponseDto
{
    public int IdAlumno { get; set; }
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public DateOnly FecNac { get; set; }
    public bool Activo { get; set; }
    public List<AlumnoMatriculaDto> Matriculas { get; set; } = new();
    public List<AlumnoTutorDto> Tutores { get; set; } = new();
}
