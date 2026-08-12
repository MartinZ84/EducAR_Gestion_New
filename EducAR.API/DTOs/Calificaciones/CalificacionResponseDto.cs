namespace EducAR.API.DTOs.Calificaciones;

public class CalificacionResponseDto
{
    public int IdCalificacion { get; set; }
    public int IdAlumno { get; set; }
    public string NombreAlumno { get; set; } = null!;
    public string ApellidoAlumno { get; set; } = null!;
    public int IdMateria { get; set; }
    public string NombreMateria { get; set; } = null!;
    public int IdPeriodoEvaluacion { get; set; }
    public string NombrePeriodo { get; set; } = null!;
    public decimal ValorCalificacion { get; set; }
    public string? Observacion { get; set; }
    public DateTime Fecha { get; set; }
}