using AutoMapper;
using GestorDeUsuarios.Application.Abstractions.UsesCases;
using GestorDeUsuarios.Application.Exceptions;
using GestorDeUsuarios.Application.Models.Requests;
using GestorDeUsuarios.Application.Models.Responses;
using GestorDeUsuarios.Domain.Abstractions.Repositories;

namespace GestorDeUsuarios.Application.UsesCases;

public class SearchUsersUseCase : ISearchUsersUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public SearchUsersUseCase(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<IEnumerable<UserResponse>> ExecuteAsync(SearchUsersRequest request)
    {
        if (IsEmptySearch(request))
            throw new InvalidSearchCriteriaException();

        var users = await _userRepository.SearchAsync(request.Name, request.Province, request.City);

        return _mapper.Map<IEnumerable<UserResponse>>(users);
    }

    private static bool IsEmptySearch(SearchUsersRequest request)
    {
        return string.IsNullOrWhiteSpace(request.Name) &&
               string.IsNullOrWhiteSpace(request.Province) &&
               string.IsNullOrWhiteSpace(request.City);
    }
}
