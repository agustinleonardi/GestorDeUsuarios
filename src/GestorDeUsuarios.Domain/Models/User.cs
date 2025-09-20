using GestorDeUsuarios.Domain.Exceptions;

namespace GestorDeUsuarios.Domain.Models;

public class User
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateTime CreationDate { get; private set; }
    public Address? Address { get; private set; }

    //Constructor para logica
    public User(string name, string email, DateTime creationDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidUserDataException(nameof(Name), "No nombre puede estar vacio");
        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidUserDataException(nameof(Email), "El email no puede estar vacio");

        Id = 0;
        Name = name;
        Email = email;
        CreationDate = creationDate;
    }
    public User(string name, string email, DateTime creationDate, Address address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidUserDataException(nameof(Name), "No nombre puede estar vacio");
        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidUserDataException(nameof(Email), "El email no puede estar vacio");
        if (address == null)
            throw new InvalidUserDataException(nameof(Address), "La direccion no puede estar vacia");

        Id = 0;
        Name = name;
        Email = email;
        CreationDate = creationDate;
        Address = address;
    }
    public void AssignAddress(Address address)
    {
        Address = address;
    }
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidUserDataException(nameof(Name), "El nombre no puede estar vacio");

        Name = name;
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidUserDataException(nameof(Email), "El email no puede estar vacio");

        Email = email;
    }

    public void UpdateAddress(Address? address)
    {
        Address = address;
    }

    public void RemoveAddress()
    {
        Address = null;
    }
}