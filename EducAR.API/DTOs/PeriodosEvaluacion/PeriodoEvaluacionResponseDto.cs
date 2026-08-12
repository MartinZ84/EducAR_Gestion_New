namespace EducAR.API.DTOs.PeriodosEvaluacion;

public class PeriodoEvaluacionResponseDto
{
    public int IdPeriodoEvaluacion { get; set; }
    public int IdCicloLectivo { get; set; }
    public int Anio { get; set; }
    public string Nombre { get; set; } = null!;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activo { get; set; }
}