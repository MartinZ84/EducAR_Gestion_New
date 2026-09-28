namespace EducAR.API.Services.Interfaces;

public interface IEmailNotificacionesService
{
    Task<bool> EnviarAsync(
        string emailRemitente,
        string nombreRemitente,
        IReadOnlyCollection<(string Email, string Nombre)> destinatarios,
        string asunto,
        string cuerpo);
}