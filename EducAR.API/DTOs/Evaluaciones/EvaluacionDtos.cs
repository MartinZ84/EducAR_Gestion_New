using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.Evaluaciones;

public class EvaluacionEditarDto
{
    [Required, MaxLength(200)] public string Titulo { get; set; } = "";
    [Required, MaxLength(4000)] public string Temario { get; set; } = "";
    [Required, MaxLength(4000)] public string Descripcion { get; set; } = "";
    public DateTime Fecha { get; set; }
}

public class EvaluacionCrearDto : EvaluacionEditarDto
{
    [Range(1, int.MaxValue)] public int IdCurso { get; set; }
    [Range(1, int.MaxValue)] public int IdMateria { get; set; }
    [Range(1, int.MaxValue)] public int IdPeriodoEvaluacion { get; set; }
}

public class NotaEvaluacionDto
{
    public int IdAlumno { get; set; }
    public decimal? Valor { get; set; }
}

public class NotasEvaluacionGuardarDto
{
    [Required] public List<NotaEvaluacionDto> Notas { get; set; } = new();
}

public record AccesoEvaluacion(int IdUsuario, int IdEscuela, bool EsAdmin);
public record ResultadoEvaluacion(int Estado, object? Datos = null);
public record ErrorNota(int IdAlumno, string Mensaje);
public record NotaGuardada(int IdAlumno, decimal? Valor, string Accion);
public record ResultadoNotas(string Mensaje, List<NotaGuardada> Resultados, List<int> CalificacionesConservadas);
