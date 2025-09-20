namespace GestorDeUsuarios.Application.Models.Responses;

public record UserResponse(
    string Name,
    string Email,
    AddressResponse? Address = null
);