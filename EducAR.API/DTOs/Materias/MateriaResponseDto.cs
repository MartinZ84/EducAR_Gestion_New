namespace EducAR.API.DTOs.Materias;

public class MateriaResponseDto
{
    public int IdMateria { get; set; }
    public int IdEscuela { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}