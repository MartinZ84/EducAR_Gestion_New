namespace EducAR.API.DTOs.Telefonos;

public class TelefonoDto
{
    public int IdTelefono { get; set; }
    public int IdAlumno { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string? Tipo { get; set; }
    public bool EsPrincipal { get; set; }
}