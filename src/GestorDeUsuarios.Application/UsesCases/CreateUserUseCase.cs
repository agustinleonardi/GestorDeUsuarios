using AutoMapper;
using GestorDeUsuarios.Application.Abstractions.UsesCases;
using GestorDeUsuarios.Application.Exceptions;
using GestorDeUsuarios.Application.Models.Requests;
using GestorDeUsuarios.Application.Models.Responses;
using GestorDeUsuarios.Domain.Abstractions.Repositories;
using GestorDeUsuarios.Domain.Abstractions.Services;
using GestorDeUsuarios.Domain.Models;


namespace GestorDeUsuarios.Application.UsesCases;

public class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    public CreateUserUseCase(IUserRepository usuarioRepository, IMapper mapper, IAddressRepository addressRepository, IAuthService authService, IEmailService emailService)
    {
        _userRepository = usuarioRepository;
        _addressRepository = addressRepository;
        _mapper = mapper;
        _authService = authService;
        _emailService = emailService;

    }

    public async Task<UserResponse> ExecuteAsync(CreateUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null) throw new UserAlreadyExistsException(request.Email);

        //hasheamos la contraseña
        var passwordHash = await _authService.HashPasswordAsync(request.Password);

        // Crear y guardar usuario - EF genera automáticamente el ID
        var user = new User(request.Name, request.Email, passwordHash, DateTime.UtcNow);
        var savedUser = await _userRepository.AddAsync(user); // Retorna con ID asignado

        // Enviar email de bienvenida
        await _emailService.SendWelcomeEmailAsync(savedUser.Email, savedUser.Name);

        // Si tiene direccion, crearlo con el UserId ya generado
        if (request.Address != null)
        {
            var address = new Address(
                savedUser.Id,
                request.Address.Street,
                request.Address.Number,
                request.Address.Province,
                request.Address.City,
                DateTime.UtcNow
            );

            var savedAddress = await _addressRepository.AddAsync(address);
            savedUser.AssignAddress(savedAddress);
        }

        return _mapper.Map<UserResponse>(savedUser);
    }

}