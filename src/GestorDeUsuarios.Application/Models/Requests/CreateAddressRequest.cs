namespace GestorDeUsuarios.Application.Models.Requests;

public record CreateAddressRequest(
    string Street,
    string Number,
    string Province,
    string City
);
