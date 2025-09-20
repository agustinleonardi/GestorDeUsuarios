using AutoMapper;
using GestorDeUsuarios.Domain.Abstractions.Repositories;
using GestorDeUsuarios.Domain.Models;
using GestorDeUsuarios.Infrastructure.Data;
using GestorDeUsuarios.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestorDeUsuarios.Infrastructure.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AddressRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Address> AddAsync(Address address)
    {
        var entity = _mapper.Map<AddressEntity>(address);
        await _context.Addresses.AddAsync(entity);
        await _context.SaveChangesAsync(); // EF asigna automáticamente el ID generado

        // Mapear de vuelta el entity con el ID generado al domain model
        return _mapper.Map<Address>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Addresses.FirstOrDefaultAsync(d => d.Id == id);
        if (entity != null)
        {
            _context.Addresses.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Address?> GetByIdAsync(int id)
    {
        var entity = await _context.Addresses.FirstOrDefaultAsync(d => d.Id == id);
        return entity == null ? null : _mapper.Map<Address>(entity);
    }

    public async Task<IEnumerable<Address>> GetByUsuarioIdAsync(int usuarioId)
    {
        var entities = await _context.Addresses
        .Where(d => d.UserId == usuarioId)
        .ToListAsync();

        return _mapper.Map<IEnumerable<Address>>(entities);
    }

    public async Task UpdateAsync(Address Address)
    {
        _context.Addresses.Update(_mapper.Map<AddressEntity>(Address));
        await _context.SaveChangesAsync();
    }
}