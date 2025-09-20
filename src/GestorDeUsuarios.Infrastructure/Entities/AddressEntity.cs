
namespace GestorDeUsuarios.Infrastructure.Entities;

public class AddressEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Street { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public UserEntity User { get; set; } = null!;

    //constructor sin parametros para que EF pueda crear instancias de la entidad sin leer datos de la base de datos
    public AddressEntity() { }

}