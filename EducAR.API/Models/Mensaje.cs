using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class Mensaje
{
    [Key]
    public int IdMensaje { get; set; }
    public int IdUsuarioRemitente { get; set; }
    public int IdUsuarioDestinat { get; set; }
    public string Asunto { get; set; } = null!;
    public string MensajeTexto { get; set; } = null!;
    public DateTime FechaEnvio { get; set; } = DateTime.Now;
    public bool Leido { get; set; } = false;
    public bool Activo { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public Usuario Remitente { get; set; } = null!;
    public Usuario Destinatario { get; set; } = null!;
}