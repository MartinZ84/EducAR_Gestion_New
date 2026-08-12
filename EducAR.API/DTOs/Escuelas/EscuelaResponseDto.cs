namespace EducAR.API.DTOs.Escuelas;

public class EscuelaResponseDto
{
    public int IdEscuela { get; set; }
    public string Nombre { get; set; } = null!;
    public string Direccion { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool Activo { get; set; }
}