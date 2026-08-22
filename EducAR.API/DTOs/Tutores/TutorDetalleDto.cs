namespace EducAR.API.DTOs.Tutores;

public class TutorDetalleDto
{
    public int IdTutor { get; set; }
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool Activo { get; set; }
    public List<AlumnoTutorDetalleDto> AlumnosAsignados { get; set; } = new();
}

public class AlumnoTutorDetalleDto
{
    public int IdAlumno { get; set; }
    public int Dni { get; set; }
    public string NombreCompleto { get; set; } = null!;
    public string CursoActual { get; set; } = null!;
}
