namespace EducAR.API.DTOs.Paginacion;

public class FiltroCursoDto : PaginacionDto
{
    public int? Grado { get; set; }
    public string? Division { get; set; }
    public string? Turno { get; set; }
    public int? IdCicloLectivo { get; set; }
}
