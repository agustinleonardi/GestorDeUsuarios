namespace GestorDeUsuarios.Application.Models.Requests;

public record UpdateUserRequest(
    string Name,
    string Email,
    UpdateAddressRequest? Address = null
);
