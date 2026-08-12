namespace EducAR.API.DTOs.Mensajes;

public class MensajeResumenDto
{
    public int IdMensaje { get; set; }
    public string NombreRemitente { get; set; } = null!;
    public string NombreDestinatario { get; set; } = null!;
    public string Asunto { get; set; } = null!;
    public DateTime FechaEnvio { get; set; }
    public bool Leido { get; set; }
}