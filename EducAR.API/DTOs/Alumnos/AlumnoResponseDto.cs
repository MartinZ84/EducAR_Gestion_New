namespace EducAR.API.DTOs.Alumnos;

public class AlumnoResponseDto
{
    public int IdAlumno { get; set; }
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public DateTime? FechaNacimiento { get; set; }
    public bool Activo { get; set; }
    public string? Calle { get; set; }
    public string? Numero { get; set; }
    public string? Piso { get; set; }
    public string? Departamento { get; set; }
    public string? Barrio { get; set; }
    public string? Localidad { get; set; }
    public string? Provincia { get; set; }
    public List<AlumnoMatriculaDto> Matriculas { get; set; } = new();
    public List<AlumnoTutorDto> Tutores { get; set; } = new();
}
