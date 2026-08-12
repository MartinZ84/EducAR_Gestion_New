namespace EducAR.API.DTOs.CiclosLectivos;

public class CicloLectivoResponseDto
{
    public int IdCicloLectivo { get; set; }
    public int IdEscuela { get; set; }
    public int Anio { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activo { get; set; }
}