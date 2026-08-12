namespace EducAR.API.DTOs.Boletines;

public class BoletinResponseDto
{
    public int IdBoletin { get; set; }
    public int IdAlumno { get; set; }
    public string NombreAlumno { get; set; } = null!;
    public string ApellidoAlumno { get; set; } = null!;
    public int IdCurso { get; set; }
    public string Curso { get; set; } = null!;
    public int IdPeriodoEvaluacion { get; set; }
    public string NombrePeriodo { get; set; } = null!;
    public string? ObservacionGeneral { get; set; }
    public decimal PromedioGeneral { get; set; }
    public DateTime FechaGeneracion { get; set; }
    public List<DetalleBoletinResponseDto> Detalle { get; set; } = new();
}

public class DetalleBoletinResponseDto
{
    public int IdMateria { get; set; }
    public string NombreMateria { get; set; } = null!;
    public decimal CalificacionFinal { get; set; }
    public string? ConceptoFinal { get; set; }
}