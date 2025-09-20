using AutoMapper;
using GestorDeUsuarios.Domain.Models;
using GestorDeUsuarios.Application.Models.Responses;

namespace GestorDeUsuarios.Application.Mappings;

public class DomainToResponseProfile : Profile
{
    public DomainToResponseProfile()
    {
        // Mapeos automáticos (nombres coinciden)
        CreateMap<Address, AddressResponse>();
        CreateMap<User, UserResponse>();
    }
}