namespace EducAR.API.DTOs.AlumnoTutor;

public class AlumnoTutorResponseDto
{
    public int IdAlumnoTutor { get; set; }
    public int IdAlumno { get; set; }
    public string NombreAlumno { get; set; } = null!;
    public string ApellidoAlumno { get; set; } = null!;
    public int IdTutor { get; set; }
    public string NombreTutor { get; set; } = null!;
    public string ApellidoTutor { get; set; } = null!;
    public string? Parentesco { get; set; }
    public bool EsResponsablePrinc { get; set; }
    public bool Activo { get; set; }
}