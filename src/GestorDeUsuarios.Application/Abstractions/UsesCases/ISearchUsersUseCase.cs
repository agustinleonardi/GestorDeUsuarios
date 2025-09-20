using GestorDeUsuarios.Application.Models.Requests;
using GestorDeUsuarios.Application.Models.Responses;

namespace GestorDeUsuarios.Application.Abstractions.UsesCases;

public interface ISearchUsersUseCase
{
    Task<IEnumerable<UserResponse>> ExecuteAsync(SearchUsersRequest request);
}

