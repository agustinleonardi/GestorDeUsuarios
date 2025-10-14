using GestorDeUsuarios.Domain.Abstractions.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;

namespace GestorDeUsuarios.Infrastructure.Services;

public class SendGridEmailService : IEmailService
{
    private readonly SendGridClient _sendGridClient;
    private readonly IConfiguration _configuration;

    public SendGridEmailService(SendGridClient sendGridClient, IConfiguration configuration)
    {
        _sendGridClient = sendGridClient;
        _configuration = configuration;
    }
    public async Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        // TODO: Implementar cuando necesites funcionalidad de reset de contraseña
        await Task.CompletedTask;
        throw new NotImplementedException("Funcionalidad de reset de contraseña no implementada aún");
    }

    public async Task SendWelcomeEmailAsync(string email, string name)
    {
        var fromEmail = Environment.GetEnvironmentVariable("SENDGRID_FROM_EMAIL") ?? "noreply@gestor-usuarios.com";
        var fromName = Environment.GetEnvironmentVariable("SENDGRID_FROM_NAME") ?? "Gestor de Usuarios";

        var from = new EmailAddress(fromEmail, fromName);
        var to = new EmailAddress(email, name);

        var subject = "Confirmación de registro - Gestor de Usuarios";
        var plainTextContent = $"Estimado/a {name},\n\nSu cuenta ha sido registrada exitosamente en nuestro sistema de gestión de usuarios.\n\nGracias por confiar en nosotros.\n\nAtentamente,\nEquipo de Gestor de Usuarios";
        var htmlContent = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #2c3e50;'>Confirmación de registro</h2>
                    <p>Estimado/a <strong>{name}</strong>,</p>
                    <p>Su cuenta ha sido registrada exitosamente en nuestro sistema de gestión de usuarios.</p>
                    <p>Gracias por confiar en nosotros.</p>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='color: #666; font-size: 14px;'>
                        Atentamente,<br>
                        <strong>Equipo de Gestor de Usuarios</strong>
                    </p>
                </div>
            </body>
            </html>";

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        // Mejorar reputación del email
        msg.AddHeader("X-Mailer", "GestorDeUsuarios");
        msg.AddHeader("X-Priority", "3");
        msg.SetClickTracking(false, false); // Desactivar tracking para evitar spam
        msg.SetOpenTracking(false); // Desactivar tracking de apertura

        var response = await _sendGridClient.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Body.ReadAsStringAsync();
            throw new InvalidOperationException($"Error enviando email: {response.StatusCode} - {errorBody}");
        }
    }
}