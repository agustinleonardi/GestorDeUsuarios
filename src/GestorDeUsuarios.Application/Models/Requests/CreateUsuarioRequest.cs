namespace GestorDeUsuarios.Application.Models.Requests;

public record CreateUserRequest(
    string Name,
    string Email,
    CreateAddressRequest? Address = null
);