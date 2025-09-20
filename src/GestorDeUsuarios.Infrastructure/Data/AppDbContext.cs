using GestorDeUsuarios.Infrastructure.Entidades;
using GestorDeUsuarios.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestorDeUsuarios.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<AddressEntity> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //aplica todas las configuraciones de entidades automaticamente
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);

    }
}