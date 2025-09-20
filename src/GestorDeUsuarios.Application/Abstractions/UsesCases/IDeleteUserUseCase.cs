using GestorDeUsuarios.Application.Models.Responses;

namespace GestorDeUsuarios.Application.Abstractions.UsesCases;

public interface IDeleteUserUseCase
{
    Task ExecuteAsync(int userId);
}