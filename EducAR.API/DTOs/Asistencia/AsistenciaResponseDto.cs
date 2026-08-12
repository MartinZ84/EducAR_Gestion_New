namespace EducAR.API.DTOs.Asistencia;

public class AsistenciaResponseDto
{
    public int IdAsistencia { get; set; }
    public int IdAlumno { get; set; }
    public string NombreAlumno { get; set; } = null!;
    public string ApellidoAlumno { get; set; } = null!;
    public int IdCurso { get; set; }
    public DateTime Fecha { get; set; }
    public bool Presente { get; set; }
}