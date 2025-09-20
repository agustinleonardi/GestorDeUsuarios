
using GestorDeUsuarios.Domain.Models;

namespace GestorDeUsuarios.Domain.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task<IEnumerable<User>> SearchAsync(string? name, string? province, string? city);
}