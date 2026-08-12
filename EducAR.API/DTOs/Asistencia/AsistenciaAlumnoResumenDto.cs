namespace EducAR.API.DTOs.Asistencia;

public class AsistenciaAlumnoResumenDto
{
    public int IdAlumno { get; set; }
    public string NombreCompleto { get; set; } = null!;
    public int TotalDias { get; set; }
    public int Presentes { get; set; }
    public int Ausentes { get; set; }
    public decimal PorcentajeAsistencia { get; set; }
}