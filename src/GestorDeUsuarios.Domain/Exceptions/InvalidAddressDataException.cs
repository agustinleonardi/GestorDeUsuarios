namespace GestorDeUsuarios.Domain.Exceptions;

public class InvalidAddressDataException : DomainException
{
    public InvalidAddressDataException(string field, string reason)
        : base($"Invalid {field}: {reason}") { }
}