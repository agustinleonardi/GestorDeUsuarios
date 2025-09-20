using GestorDeUsuarios.Domain.Models;

namespace GestorDeUsuarios.Domain.Abstractions.Repositories;

public interface IAddressRepository
{
    Task<Address?> GetByIdAsync(int id);
    Task<IEnumerable<Address>> GetByUsuarioIdAsync(int userId);
    Task<Address> AddAsync(Address address); // ✅ Ahora retorna el Address con ID
    Task UpdateAsync(Address address);
    Task DeleteAsync(int id);
}