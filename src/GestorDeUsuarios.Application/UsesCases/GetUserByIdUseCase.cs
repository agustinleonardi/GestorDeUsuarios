using AutoMapper;
using GestorDeUsuarios.Application.Exceptions;
using GestorDeUsuarios.Application.Models.Responses;
using GestorDeUsuarios.Domain.Abstractions.Repositories;

namespace GestorDeUsuarios.Application.Abstractions.UsesCases;

public class GetUserByIdUseCase : IGetUserByIdUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdUseCase(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<UserResponse> ExecuteAsync(int userId)
    {
        var existingUser = await _userRepository.GetByIdAsync(userId);
        if (existingUser == null)
            throw new UserNotFoundApplicationException(userId);

        return _mapper.Map<UserResponse>(existingUser);
    }
}