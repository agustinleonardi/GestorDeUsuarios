namespace GestorDeUsuarios.Domain.Exceptions;

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(int userId) : base($"El usuario con ID: {userId} no existe") { }
}