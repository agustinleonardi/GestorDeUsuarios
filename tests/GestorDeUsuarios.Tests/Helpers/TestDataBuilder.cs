using GestorDeUsuarios.Domain.Models;
using GestorDeUsuarios.Application.Models.Requests;
using GestorDeUsuarios.Application.Models.Responses;

namespace GestorDeUsuarios.Tests.Helpers;

public static class TestDataBuilder
{
    /// <summary>
    /// Crea un request válido para crear usuario sin dirección
    /// Útil para tests básicos de creación de usuarios
    /// </summary>
    public static CreateUserRequest CreateValidUserRequest(
        string name = "Juan Pérez",
        string email = "juan@example.com",
        CreateAddressRequest? address = null)
    {
        return new CreateUserRequest(name, email, address);
    }

    /// <summary>
    /// Crea un request válido para crear usuario CON dirección incluida
    /// Útil para tests que necesitan verificar la creación de usuario + dirección
    /// </summary>
    public static CreateUserRequest CreateValidUserRequestWithAddress(
        string name = "Juan Pérez",
        string email = "juan@example.com")
    {
        var addressRequest = new CreateAddressRequest("Av. Corrientes", "1234", "Buenos Aires", "CABA");
        return new CreateUserRequest(name, email, addressRequest);
    }

    /// <summary>
    /// Crea un request válido para actualizar usuario sin dirección
    /// Útil para tests de actualización básica de datos de usuario
    /// </summary>
    public static UpdateUserRequest CreateValidUpdateUserRequest(
        string name = "Juan Carlos Pérez",
        string email = "juan.carlos@example.com",
        UpdateAddressRequest? address = null)
    {
        return new UpdateUserRequest(name, email, address);
    }

    /// <summary>
    /// Crea un request válido para actualizar usuario CON dirección nueva
    /// Útil para tests de actualización que incluyen crear una nueva dirección
    /// </summary>
    public static UpdateUserRequest CreateValidUpdateUserRequestWithAddress(
        string name = "Juan Carlos Pérez",
        string email = "juan.carlos@example.com")
    {
        var addressRequest = new UpdateAddressRequest("Av. Santa Fe", "5678", "Buenos Aires", "Palermo");
        return new UpdateUserRequest(name, email, addressRequest);
    }

    /// <summary>
    /// Crea un request válido para actualizar usuario CON dirección existente
    /// Útil para tests de actualización que modifican una dirección que ya existe
    /// </summary>
    public static UpdateUserRequest CreateValidUpdateUserRequestWithAddressId(
        string name = "Juan Carlos Pérez",
        string email = "juan.carlos@example.com")
    {
        var addressRequest = new UpdateAddressRequest("Av. Santa Fe", "5678", "Buenos Aires", "Palermo");
        return new UpdateUserRequest(name, email, addressRequest);
    }

    /// <summary>
    /// Crea una entidad User válida para usar en tests
    /// Útil como mock de usuario devuelto por el repositorio
    /// </summary>
    public static User CreateValidUser(
        string name = "Juan Pérez",
        string email = "juan@example.com")
    {
        return new User(name, email, DateTime.UtcNow);
    }

    /// <summary>
    /// Crea una entidad User válida CON ID específico (usa reflection)
    /// Útil para simular usuarios existentes en BD con ID conocido
    /// </summary>
    public static User CreateValidUserWithId(int id)
    {
        var user = CreateValidUser();
        // Usar reflection para setear el Id ya que es private set
        typeof(User).GetProperty("Id")?.SetValue(user, id);
        return user;
    }

    /// <summary>
    /// Crea una entidad User válida YA CON dirección asignada
    /// Útil para tests que necesitan usuarios que ya tienen dirección
    /// </summary>
    public static User CreateValidUserWithAddress(
        string name = "Juan Pérez",
        string email = "juan@example.com")
    {
        var user = CreateValidUser(name, email);
        var address = new Address(user.Id, "Av. Corrientes", "1234", "Buenos Aires", "CABA", DateTime.UtcNow);
        user.AssignAddress(address);
        return user;
    }

    /// <summary>
    /// Crea una entidad Address válida para tests
    /// Útil como mock de dirección devuelta por el repositorio
    /// </summary>
    public static Address CreateValidAddress(int userId = 1)
    {
        return new Address(userId, "Av. Corrientes", "1234", "Buenos Aires", "CABA", DateTime.UtcNow);
    }

    /// <summary>
    /// Crea una entidad Address válida CON ID específico (usa reflection)
    /// Útil para simular direcciones existentes en BD con ID conocido
    /// </summary>
    public static Address CreateValidAddressWithId(int id, int userId = 1)
    {
        var address = CreateValidAddress(userId);
        // Usar reflection para setear el Id ya que es private set
        typeof(Address).GetProperty("Id")?.SetValue(address, id);
        return address;
    }

    /// <summary>
    /// Crea una lista de usuarios válidos para tests
    /// Útil para tests de búsqueda/listado que devuelven múltiples usuarios
    /// </summary>
    public static List<User> CreateUserList()
    {
        return new List<User>
        {
            CreateValidUser("Juan Pérez", "juan@example.com"),
            CreateValidUser("María García", "maria@example.com"),
            CreateValidUser("Carlos López", "carlos@example.com")
        };
    }

    /// <summary>
    /// Crea un request válido para buscar usuarios con filtros opcionales
    /// Útil para tests de búsqueda con diferentes combinaciones de filtros
    /// </summary>
    public static SearchUsersRequest CreateSearchRequest(
        string? name = null,
        string? city = null,
        string? province = null)
    {
        return new SearchUsersRequest(name, province, city);
    }

    /// <summary>
    /// Crea un response válido de usuario para verificar en tests
    /// Útil como valor esperado en assertions de los use cases
    /// </summary>
    public static UserResponse CreateValidUserResponse(
        string name = "Juan Pérez",
        string email = "juan@example.com",
        AddressResponse? address = null)
    {
        return new UserResponse(name, email, address);
    }

    /// <summary>
    /// Crea un response válido de dirección para verificar en tests
    /// Útil como parte del UserResponse cuando incluye dirección
    /// </summary>
    public static AddressResponse CreateValidAddressResponse()
    {
        return new AddressResponse("Av. Corrientes", "1234", "Buenos Aires", "CABA");
    }
}