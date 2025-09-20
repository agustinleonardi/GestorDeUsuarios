using GestorDeUsuarios.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestorDeUsuarios.Infrastructure.Entidades.Configuraciones;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.CreationDate).IsRequired();

        // Configurar relación 1:1 con Address - permite navegación bidireccional y uso de Include()
        builder.HasOne(u => u.Address)
            .WithOne(a => a.User)
            .HasForeignKey<AddressEntity>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Crear índice único en Email para evitar duplicados y mejorar performance en consultas
        builder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("IX_Users_Email");

        //
        builder.HasIndex(u => u.Name).HasDatabaseName("IX_Users_Name");

    }
}