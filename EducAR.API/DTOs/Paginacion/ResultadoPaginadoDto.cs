namespace EducAR.API.DTOs.Paginacion;

public class ResultadoPaginadoDto<T>
{
    public int PaginaActual { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }
    public int RegistrosPorPagina { get; set; }
    public bool TienePaginaAnterior => PaginaActual > 1;
    public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
    public List<T> Datos { get; set; } = new();
}