using GestorDeUsuarios.Domain.Exceptions;

namespace GestorDeUsuarios.Domain.Models;

public class Address
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Street { get; private set; } = string.Empty;
    public string Number { get; private set; } = string.Empty;
    public string Province { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;

    public DateTime CreationDate { get; private set; }

    public Address(int userId, string street, string number, string province, string city, DateTime creationDate)
    {
        if (userId <= 0) throw new InvalidAddressDataException(nameof(userId), "La direccion debe tener un usuario asignado");
        if (string.IsNullOrWhiteSpace(street))
            throw new InvalidAddressDataException(nameof(street), "La calle no puede estar vacia");
        if (string.IsNullOrWhiteSpace(number))
            throw new InvalidAddressDataException(nameof(number), "El numero de la calle no puede estar vacio");
        if (string.IsNullOrWhiteSpace(province))
            throw new InvalidAddressDataException(nameof(province), "La provincia no puede estar vacia");
        if (string.IsNullOrWhiteSpace(city))
            throw new InvalidAddressDataException(nameof(city), "La ciudad no puede estar vacia");


        Id = 0;
        UserId = userId;
        Street = street;
        Number = number;
        Province = province;
        City = city;
        CreationDate = creationDate;
    }

    public void UpdateStreet(string street)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new InvalidAddressDataException(nameof(street), "La calle no puede estar vacia");

        Street = street;
    }

    public void UpdateNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new InvalidAddressDataException(nameof(number), "El numero de la calle no puede estar vacio");

        Number = number;
    }

    public void UpdateProvince(string province)
    {
        if (string.IsNullOrWhiteSpace(province))
            throw new InvalidAddressDataException(nameof(province), "La provincia no puede estar vacia");

        Province = province;
    }

    public void UpdateCity(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new InvalidAddressDataException(nameof(city), "La ciudad no puede estar vacia");

        City = city;
    }
}