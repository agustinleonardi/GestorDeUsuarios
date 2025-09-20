using GestorDeUsuarios.Application.Models.Requests;
using GestorDeUsuarios.Application.Models.Responses;

namespace GestorDeUsuarios.Application.Abstractions.UsesCases;

public interface IUpdateUserUseCase
{
    Task<UserResponse> ExecuteAsync(int userId, UpdateUserRequest request);
}