namespace EducAR.API.DTOs.Asistencia;

public class AsistenciaPorFechaResponseDto
{
    public DateTime Fecha { get; set; }
    public int IdCurso { get; set; }
    public int TotalAlumnos { get; set; }
    public int Presentes { get; set; }
    public int Ausentes { get; set; }
    public List<AsistenciaResponseDto> Detalle { get; set; } = new();
}