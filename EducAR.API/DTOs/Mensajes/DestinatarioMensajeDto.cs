namespace EducAR.API.DTOs.Mensajes;

public class DestinatarioMensajeDto
{
    public int IdUsuario { get; set; }
    public int IdAlumno { get; set; }
    public string NombreCompleto { get; set; } = null!;
    public string NombreAlumno { get; set; } = null!;
    public string Rol { get; set; } = null!;
}
