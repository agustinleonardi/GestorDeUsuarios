using AutoMapper;
using GestorDeUsuarios.Domain.Models;
using GestorDeUsuarios.Infrastructure.Entities;

namespace GestorDeUsuarios.Infrastructure.Mappings;

public class DomainToEntityProfile : Profile
{
    public DomainToEntityProfile()
    {
        CreateMap<User, UserEntity>();
        CreateMap<UserEntity, User>()
            .ConstructUsing(entity =>
                // Operador ternario: condición ? valorSiTrue : valorSiFalse
                entity.Address != null
                    ? new User(entity.Name, entity.Email, entity.CreationDate,
                              new Address(entity.Address.UserId, entity.Address.Street,
                                        entity.Address.Number, entity.Address.Province,
                                        entity.Address.City, entity.Address.CreationDate))
                    : new User(entity.Name, entity.Email, entity.CreationDate));
        CreateMap<Address, AddressEntity>().ReverseMap();
    }
}