namespace GestorDeUsuarios.Domain.Abstractions.Services;

public interface IEmailService
{
    //Enviar email de bienvenida
    Task SendWelcomeEmailAsync(string email, string name);

    //email para cambiar de contraseña
    Task SendPasswordResetEmailAsync(string email, string resetToken);
}