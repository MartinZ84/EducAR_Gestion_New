namespace EducAR.API.DTOs.Paginacion;

public class PaginacionDto
{
    private const int MaxCantidad = 50;
    private int _cantidad = 10;

    public int Pagina { get; set; } = 1;
    public int Cantidad
    {
        get => _cantidad;
        set => _cantidad = value > MaxCantidad ? MaxCantidad : value;
    }
}