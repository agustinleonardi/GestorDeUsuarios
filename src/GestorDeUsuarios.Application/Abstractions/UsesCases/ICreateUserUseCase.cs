
using GestorDeUsuarios.Application.Models.Requests;
using GestorDeUsuarios.Application.Models.Responses;

namespace GestorDeUsuarios.Application.Abstractions.UsesCases;

public interface ICreateUserUseCase
{
    Task<UserResponse> ExecuteAsync(CreateUserRequest createUsuarioRequest);
}
