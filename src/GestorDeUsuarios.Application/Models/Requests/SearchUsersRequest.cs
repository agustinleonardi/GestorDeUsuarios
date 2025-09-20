namespace GestorDeUsuarios.Application.Models.Requests;

public record SearchUsersRequest(string? Name, string? Province, string? City);