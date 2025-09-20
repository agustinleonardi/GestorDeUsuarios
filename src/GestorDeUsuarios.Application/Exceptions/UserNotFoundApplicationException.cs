namespace GestorDeUsuarios.Application.Exceptions;

public class UserNotFoundApplicationException : ApplicationException
{
    public UserNotFoundApplicationException(int userId)
        : base($"Usuario con ID {userId} no encontrado") { }

    public UserNotFoundApplicationException(string message)
        : base(message) { }
}