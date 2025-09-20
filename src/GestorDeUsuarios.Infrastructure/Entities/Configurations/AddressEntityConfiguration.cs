using GestorDeUsuarios.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestorDeUsuarios.Infrastructure.Entidades.Configuraciones;

public class AddressEntityConfiguration : IEntityTypeConfiguration<AddressEntity>
{
    public void Configure(EntityTypeBuilder<AddressEntity> builder)
    {
        builder.ToTable("Addresses");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.UserId).IsRequired();
        builder.Property(d => d.Street).IsRequired().HasMaxLength(100);
        builder.Property(d => d.Number).IsRequired().HasMaxLength(10);
        builder.Property(d => d.Province).IsRequired().HasMaxLength(50);
        builder.Property(d => d.City).IsRequired().HasMaxLength(50);
        builder.Property(d => d.CreationDate).IsRequired();

        // Configurar la relación con User (lado Address de la relación 1:1)
        builder.HasOne(a => a.User)
            .WithOne(u => u.Address)
            .HasForeignKey<AddressEntity>(a => a.UserId);
    }
}