namespace GestorDeUsuarios.Application.Models.Requests;

public record UpdateAddressRequest(
    string Calle,
    string Numero,
    string Provincia,
    string Ciudad,
    int? Id = null
);