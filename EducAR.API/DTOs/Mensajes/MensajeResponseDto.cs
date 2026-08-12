namespace EducAR.API.DTOs.Mensajes;

public class MensajeResponseDto
{
    public int IdMensaje { get; set; }
    public int IdUsuarioRemitente { get; set; }
    public string NombreRemitente { get; set; } = null!;
    public int IdUsuarioDestinat { get; set; }
    public string NombreDestinatario { get; set; } = null!;
    public string Asunto { get; set; } = null!;
    public string MensajeTexto { get; set; } = null!;
    public DateTime FechaEnvio { get; set; }
    public bool Leido { get; set; }
}