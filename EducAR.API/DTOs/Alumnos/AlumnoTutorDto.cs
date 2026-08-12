namespace EducAR.API.DTOs.Alumnos;

public class AlumnoTutorDto
{
    public int IdAlumnoTutor { get; set; }
    public int IdTutor { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string? Parentesco { get; set; }
    public bool EsResponsablePrinc { get; set; }
    public bool Activo { get; set; }
}