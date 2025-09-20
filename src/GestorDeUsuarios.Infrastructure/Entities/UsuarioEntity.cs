using System.Runtime;

namespace GestorDeUsuarios.Infrastructure.Entities;

public class UserEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public AddressEntity? Address { get; set; } // Relación 1:1 (opcional)

    //constructor sin parametros para que EF pueda crear instancias de la entidad sin leer datos de la base de datos
    public UserEntity() { }
}
