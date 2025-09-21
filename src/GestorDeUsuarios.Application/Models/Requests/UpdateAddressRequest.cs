namespace GestorDeUsuarios.Application.Models.Requests;

public record UpdateAddressRequest(
    string Street,
    string Number,
    string Province,
    string City
);