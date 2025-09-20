namespace GestorDeUsuarios.Domain.Exceptions;

public class InvalidUserDataException : DomainException
{
    public InvalidUserDataException(string field, string reason)
        : base($"Invalid {field}: {reason}") { }
}