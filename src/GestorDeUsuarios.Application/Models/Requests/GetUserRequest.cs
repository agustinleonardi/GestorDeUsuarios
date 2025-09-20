namespace GestorDeUsuarios.Application.Models.Requests;

public record GetUserRequest(
    string? UserName = null,
    string? Province = null,
    string? City = null
);