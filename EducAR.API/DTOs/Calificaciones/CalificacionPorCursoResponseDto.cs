namespace EducAR.API.DTOs.Calificaciones;

public class CalificacionPorCursoResponseDto
{
    public int IdCurso { get; set; }
    public int IdMateria { get; set; }
    public string NombreMateria { get; set; } = null!;
    public int IdPeriodoEvaluacion { get; set; }
    public string NombrePeriodo { get; set; } = null!;
    public List<CalificacionResponseDto> Calificaciones { get; set; } = new();
}