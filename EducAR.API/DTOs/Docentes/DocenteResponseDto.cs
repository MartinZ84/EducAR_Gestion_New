namespace EducAR.API.DTOs.Docentes;

public class DocenteResponseDto
{
    public int IdDocente { get; set; }
    public int IdUsuario { get; set; }
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string NombreUsuario { get; set; } = null!;
    public bool Activo { get; set; }
}