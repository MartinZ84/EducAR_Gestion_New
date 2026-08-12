namespace EducAR.API.DTOs.Matriculas;

public class MatriculaAsignacionResultadoDto
{
    public int TotalSolicitados { get; set; }
    public int Matriculados { get; set; }
    public int YaMatriculados { get; set; }
    public int Conflictos { get; set; }
    public List<MatriculaAsignacionDetalleDto> Detalle { get; set; } = new();
}

public class MatriculaAsignacionDetalleDto
{
    public int IdAlumno { get; set; }
    public string Apellido { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Resultado { get; set; } = null!;
    public string? Mensaje { get; set; }
    public int? IdMatricula { get; set; }
}
