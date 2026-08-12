namespace EducAR.API.DTOs.Paginacion;

public class FiltroPersonaDto : PaginacionDto
{
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public int? Dni { get; set; }
}
