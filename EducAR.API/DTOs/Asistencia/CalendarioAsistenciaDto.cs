namespace EducAR.API.DTOs.Asistencia;

public class CalendarioAsistenciaDto
{
    public int IdCurso { get; set; }
    public string Curso { get; set; } = null!;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int Pendientes { get; set; }
    public List<DiaAsistenciaDto> Dias { get; set; } = new();
}

public class DiaAsistenciaDto
{
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = null!;
    public string? Motivo { get; set; }
}

public class PendientesAsistenciaDto
{
    public int Cantidad { get; set; }
    public int CursosConPendientes { get; set; }
}
