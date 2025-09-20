namespace GestorDeUsuarios.Application.Exceptions;

public class UserAlreadyExistsException : ApplicationException
{
    public UserAlreadyExistsException(string message) : base(message) { }

}