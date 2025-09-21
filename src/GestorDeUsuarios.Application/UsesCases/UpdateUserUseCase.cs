using AutoMapper;
using GestorDeUsuarios.Application.Abstractions.UsesCases;
using GestorDeUsuarios.Application.Exceptions;
using GestorDeUsuarios.Application.Models.Requests;
using GestorDeUsuarios.Application.Models.Responses;
using GestorDeUsuarios.Domain.Abstractions.Repositories;
using GestorDeUsuarios.Domain.Models;

namespace GestorDeUsuarios.Application.UsesCases;

public class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IMapper _mapper;

    public UpdateUserUseCase(IUserRepository userRepository, IAddressRepository addressRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _addressRepository = addressRepository;
        _mapper = mapper;
    }

    public async Task<UserResponse> ExecuteAsync(int userId, UpdateUserRequest request)
    {
        // Obtengo el usuario existente
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new UserNotFoundApplicationException(userId);

        //Verifico si el email ya está en uso por otro usuario
        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existingUserWithEmail = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUserWithEmail != null && existingUserWithEmail.Id != userId)
                throw new UserAlreadyExistsException(request.Email);
        }

        // Actualizo datos básicos del usuario
        user.UpdateName(request.Name);
        user.UpdateEmail(request.Email);

        // Aca actualizo la dirección
        await HandleAddressUpdateAsync(user, request.Address);

        // Guardar cambios
        await _userRepository.UpdateAsync(user);

        return _mapper.Map<UserResponse>(user);
    }

    private async Task HandleAddressUpdateAsync(User user, UpdateAddressRequest? addressRequest)
    {
        if (addressRequest == null)
        {
            // Si no se proporciona dirección, eliminarla si existía
            if (user.Address != null)
            {
                user.RemoveAddress();
            }
            return;
        }

        if (user.Address == null)
        {
            // Crear nueva dirección
            var newAddress = new Address(
                user.Id,
                addressRequest.Street,
                addressRequest.Number,
                addressRequest.Province,
                addressRequest.City,
                DateTime.UtcNow
            );

            var savedAddress = await _addressRepository.AddAsync(newAddress);
            user.UpdateAddress(savedAddress);
        }
        else
        {
            // Actualizar dirección existente
            user.Address.UpdateCalle(addressRequest.Street);
            user.Address.UpdateNumero(addressRequest.Number);
            user.Address.UpdateProvincia(addressRequest.Province);
            user.Address.UpdateCiudad(addressRequest.City);
        }
    }
}