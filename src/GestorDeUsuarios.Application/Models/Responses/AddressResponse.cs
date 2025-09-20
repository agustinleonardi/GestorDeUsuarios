namespace GestorDeUsuarios.Application.Models.Responses;

public record AddressResponse(
    string Street,
    string Number,
    string Province,
    string City
);