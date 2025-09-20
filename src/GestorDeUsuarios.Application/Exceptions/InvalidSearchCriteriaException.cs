namespace GestorDeUsuarios.Application.Exceptions;

public class InvalidSearchCriteriaException : ApplicationException
{
    public InvalidSearchCriteriaException(string message) : base(message) { }
    public InvalidSearchCriteriaException() : base("Al menos un criterio de busqueda debe tener valor") { }
}