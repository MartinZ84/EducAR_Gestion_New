using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.Mensajes;

public class MensajeCreateDto
{
    [Required(ErrorMessage = "El destinatario es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un destinatario válido.")]
    public int IdUsuarioDestinat { get; set; }

    [Required(ErrorMessage = "El asunto es obligatorio.")]
    [MaxLength(200, ErrorMessage = "El asunto no puede superar los 200 caracteres.")]
    public string Asunto { get; set; } = null!;

    [Required(ErrorMessage = "El mensaje es obligatorio.")]
    [MaxLength(2000, ErrorMessage = "El mensaje no puede superar los 2000 caracteres.")]
    public string MensajeTexto { get; set; } = null!;
}
