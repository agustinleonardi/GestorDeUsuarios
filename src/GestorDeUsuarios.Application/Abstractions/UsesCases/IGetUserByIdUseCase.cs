using GestorDeUsuarios.Application.Models.Responses;

namespace GestorDeUsuarios.Application.Abstractions.UsesCases;

public interface IGetUserByIdUseCase
{
    Task<UserResponse> ExecuteAsync(int userId);
}