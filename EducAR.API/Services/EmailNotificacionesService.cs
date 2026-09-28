using System.Net;
using System.Net.Mail;
using EducAR.API.Services.Interfaces;

namespace EducAR.API.Services;

public class EmailNotificacionesService : IEmailNotificacionesService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailNotificacionesService> _logger;

    public EmailNotificacionesService(
        IConfiguration configuration,
        ILogger<EmailNotificacionesService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> EnviarAsync(
        string emailRemitente,
        string nombreRemitente,
        IReadOnlyCollection<(string Email, string Nombre)> destinatarios,
        string asunto,
        string cuerpo)
    {
        var host = _configuration["Email:SmtpHost"];
        var fromAddress = _configuration["Email:FromAddress"];
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(fromAddress))
        {
            _logger.LogWarning("No se enviaron emails porque falta la configuración SMTP.");
            return false;
        }

        var port = int.TryParse(_configuration["Email:SmtpPort"], out var configuredPort)
            ? configuredPort
            : 587;
        var enableSsl = bool.TryParse(_configuration["Email:EnableSsl"], out var configuredSsl) && configuredSsl;
        var allSent = true;

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            UseDefaultCredentials = false,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        var username = _configuration["Email:Username"];
        var password = _configuration["Email:Password"];
        if (!string.IsNullOrWhiteSpace(username))
            client.Credentials = new NetworkCredential(username, password);

        foreach (var destinatario in destinatarios)
        {
            try
            {
                using var mail = new MailMessage
                {
                    From = new MailAddress(fromAddress, _configuration["Email:FromName"] ?? "EducAR"),
                    Subject = asunto,
                    Body = cuerpo,
                    IsBodyHtml = false
                };
                mail.ReplyToList.Add(new MailAddress(emailRemitente, nombreRemitente));
                mail.To.Add(new MailAddress(destinatario.Email, destinatario.Nombre));
                await client.SendMailAsync(mail);
            }
            catch (Exception exception)
            {
                allSent = false;
                _logger.LogError(exception, "No se pudo enviar una notificación por email.");
            }
        }

        return allSent;
    }
}