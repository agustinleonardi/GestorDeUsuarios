using AutoMapper;
using GestorDeUsuarios.Domain.Abstractions.Repositories;
using GestorDeUsuarios.Domain.Models;
using GestorDeUsuarios.Infrastructure.Data;
using GestorDeUsuarios.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestorDeUsers.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UserRepository(AppDbContext appDbContext, IMapper mapper)
    {
        _context = appDbContext;
        _mapper = mapper;
    }
    public async Task<User> AddAsync(User user)
    {
        var entity = _mapper.Map<UserEntity>(user);
        await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync(); // EF asigna automáticamente el ID generado a entity.Id

        // Mapear de vuelta el entity con el ID generado al domain model
        return _mapper.Map<User>(entity);
    }


    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Users.FindAsync(id);
        if (entity != null)
        {
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var entity = await _context.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email);
        if (entity == null) return null;
        return _mapper.Map<User>(entity);

    }

    public async Task<User?> GetByIdAsync(int id)
    {
        var entity = await _context.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Id == id);
        if (entity == null) return null;
        return _mapper.Map<User>(entity);
    }

    public async Task<IEnumerable<User>> SearchAsync(string? name, string? province, string? city)
    {
        var query = _context.Users.Include(u => u.Address).AsQueryable();

        // Filtro 1: Por nombre (si se proporciona)
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(u => u.Name.Contains(name));
        }

        // Filtro 2: Por provincia (si se proporciona)
        if (!string.IsNullOrWhiteSpace(province))
        {
            query = query.Where(u => u.Address != null && u.Address.Province.Contains(province));
        }

        // Filtro 3: Por ciudad (si se proporciona)
        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(u => u.Address != null && u.Address.City.Contains(city));
        }

        // Ejecutar la query y convertir a List
        var entities = await query.ToListAsync();

        // Mapear entities a domain models
        return _mapper.Map<IEnumerable<User>>(entities);
    }

    public async Task UpdateAsync(User user)
    {
        var userEntity = _mapper.Map<UserEntity>(user);

        // Obtener la entidad existente que ya está siendo trackeada
        var existingEntity = await _context.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Id == userEntity.Id);

        if (existingEntity != null)
        {
            // Actualizar los valores de la entidad existente
            _context.Entry(existingEntity).CurrentValues.SetValues(userEntity);

            // Actualizar la dirección si existe
            if (userEntity.Address != null)
            {
                if (existingEntity.Address != null)
                {
                    _context.Entry(existingEntity.Address).CurrentValues.SetValues(userEntity.Address);
                }
                else
                {
                    existingEntity.Address = userEntity.Address;
                }
            }
            else if (existingEntity.Address != null)
            {
                _context.Remove(existingEntity.Address);
                existingEntity.Address = null;
            }
        }
        else
        {
            // Si no existe una entidad trackeada, usar Update
            _context.Users.Update(userEntity);
        }

        await _context.SaveChangesAsync();
    }
}