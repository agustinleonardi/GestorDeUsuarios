using System.Net;
using FluentAssertions;
using GestorDeUsuarios.Application.Models.Requests;
using GestorDeUsuarios.Application.Models.Responses;
using GestorDeUsuarios.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GestorDeUsuarios.Tests.IntegrationTests;

public class UsuariosControllerIntegrationTests : BaseIntegrationTest
{
    public UsuariosControllerIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    // Test - POST /api/users con datos válidos debe crear usuario y retornar 201
    [Fact]
    public async Task POST_CreateUser_WithValidData_ShouldReturn201AndCreateUser()
    {
        // Arrange: Crear request válido para nuevo usuario
        var createRequest = new CreateUserRequest(
            "Juan Pérez",
            "juan.perez@test.com",
            new CreateAddressRequest("Av. Corrientes", "1234", "Buenos Aires", "CABA")
        );

        // Act: Enviar POST al endpoint
        var response = await _client.PostAsync("/api/users", CreateJsonContent(createRequest));

        // Assert: Verificar respuesta HTTP y base de datos
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var userResponse = await GetResponseContent<UserResponse>(response);
        userResponse.Should().NotBeNull();
        userResponse!.Name.Should().Be("Juan Pérez");
        userResponse.Email.Should().Be("juan.perez@test.com");

        // Verificar que se creó en la base de datos
        var userInDb = await _dbContext.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Email == "juan.perez@test.com");

        userInDb.Should().NotBeNull();
        userInDb!.Name.Should().Be("Juan Pérez");
        userInDb.Address.Should().NotBeNull();
        userInDb.Address!.Street.Should().Be("Av. Corrientes");
    }

    // Test - POST /api/users con datos inválidos debe retornar 400
    [Fact]
    public async Task POST_CreateUser_WithInvalidData_ShouldReturn400()
    {
        // Arrange: Crear request inválido (email vacío)
        var invalidRequest = new CreateUserRequest("", "email-invalido", null);

        // Act: Enviar POST con datos inválidos
        var response = await _client.PostAsync("/api/users", CreateJsonContent(invalidRequest));

        // Assert: Verificar error de validación
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Verificar que NO se creó en la base de datos
        var userCount = await _dbContext.Users.CountAsync();
        userCount.Should().Be(0);
    }

    // Test - POST /api/users con email duplicado debe retornar 409
    [Fact]
    public async Task POST_CreateUser_WithDuplicateEmail_ShouldReturn409()
    {
        // Arrange: Crear usuario existente en BD y request con mismo email
        var existingUser = new UserEntity
        {
            Name = "María García",
            Email = "maria@test.com",
            CreationDate = DateTime.UtcNow
        };
        _dbContext.Users.Add(existingUser);
        await _dbContext.SaveChangesAsync();

        var duplicateRequest = new CreateUserRequest("Juan Pérez", "maria@test.com", null);

        // Act: Intentar crear usuario con email duplicado
        var response = await _client.PostAsync("/api/users", CreateJsonContent(duplicateRequest));

        // Assert: Verificar conflicto y que no se creó segundo usuario
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var userCount = await _dbContext.Users.CountAsync();
        userCount.Should().Be(1); // Solo el usuario original
    }

    // Test - GET /api/users/{id} con ID existente debe retornar 200 y usuario
    [Fact]
    public async Task GET_GetUserById_WithExistingId_ShouldReturn200AndUser()
    {
        // Arrange: Crear usuario en BD con dirección
        var user = new UserEntity
        {
            Name = "Carlos López",
            Email = "carlos@test.com",
            CreationDate = DateTime.UtcNow
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var address = new AddressEntity
        {
            UserId = user.Id,
            Street = "Av. Santa Fe",
            Number = "5678",
            Province = "Buenos Aires",
            City = "Palermo",
            CreationDate = DateTime.UtcNow,
            User = user
        };
        _dbContext.Addresses.Add(address);
        user.Address = address;
        await _dbContext.SaveChangesAsync();

        // Act: Solicitar usuario por ID
        var response = await _client.GetAsync($"/api/users/{user.Id}");

        // Assert: Verificar respuesta exitosa
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var userResponse = await GetResponseContent<UserResponse>(response);
        userResponse.Should().NotBeNull();
        userResponse!.Name.Should().Be("Carlos López");
        userResponse.Email.Should().Be("carlos@test.com");
        userResponse.Address.Should().NotBeNull();
        userResponse.Address!.Street.Should().Be("Av. Santa Fe");
    }

    // Test - GET /api/users/{id} con ID inexistente debe retornar 404
    [Fact]
    public async Task GET_GetUserById_WithNonExistentId_ShouldReturn404()
    {
        // Arrange: ID que no existe
        var nonExistentId = 999;

        // Act: Solicitar usuario inexistente
        var response = await _client.GetAsync($"/api/users/{nonExistentId}");

        // Assert: Verificar error 404
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Test - PUT /api/users/{id} con datos válidos debe actualizar usuario y retornar 200
    [Fact]
    public async Task PUT_UpdateUser_WithValidData_ShouldReturn200AndUpdateUser()
    {
        // Arrange: Crear usuario existente
        var user = new UserEntity
        {
            Name = "Ana Rodríguez",
            Email = "ana@test.com",
            CreationDate = DateTime.UtcNow
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Limpiar el contexto para evitar conflictos de tracking
        _dbContext.ChangeTracker.Clear();

        var updateRequest = new UpdateUserRequest(
            "Ana María Rodríguez",
            "ana.maria@test.com",
            new UpdateAddressRequest("Av. Libertador", "999", "Buenos Aires", "Recoleta")
        );

        // Act: Actualizar usuario
        var response = await _client.PutAsync($"/api/users/{user.Id}", CreateJsonContent(updateRequest));

        // Assert: Verificar actualización exitosa
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var userResponse = await GetResponseContent<UserResponse>(response);
        userResponse.Should().NotBeNull();
        userResponse!.Name.Should().Be("Ana María Rodríguez");
        userResponse.Email.Should().Be("ana.maria@test.com");

        // Verificar cambios en BD
        var updatedUser = await _dbContext.Users
            .Include(u => u.Address)
            .FirstAsync(u => u.Id == user.Id);

        updatedUser.Name.Should().Be("Ana María Rodríguez");
        updatedUser.Email.Should().Be("ana.maria@test.com");
        updatedUser.Address.Should().NotBeNull();
        updatedUser.Address!.Street.Should().Be("Av. Libertador");
    }

    // Test - PUT /api/users/{id} con ID inexistente debe retornar 404
    [Fact]
    public async Task PUT_UpdateUser_WithNonExistentId_ShouldReturn404()
    {
        // Arrange: ID inexistente y request de actualización
        var nonExistentId = 999;
        var updateRequest = new UpdateUserRequest("Test User", "test@test.com", null);

        // Act: Intentar actualizar usuario inexistente
        var response = await _client.PutAsync($"/api/users/{nonExistentId}", CreateJsonContent(updateRequest));

        // Assert: Verificar error 404
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Test - DELETE /api/users/{id} con ID existente debe eliminar usuario y retornar 204
    [Fact]
    public async Task DELETE_DeleteUser_WithExistingId_ShouldReturn204AndDeleteUser()
    {
        // Arrange: Crear usuario existente
        var user = new UserEntity
        {
            Name = "Pedro González",
            Email = "pedro@test.com",
            CreationDate = DateTime.UtcNow
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act: Eliminar usuario
        var response = await _client.DeleteAsync($"/api/users/{user.Id}");

        // Assert: Verificar eliminación exitosa
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verificar que se eliminó de BD
        var userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        userInDb.Should().BeNull();
    }

    // Test - DELETE /api/users/{id} con ID inexistente debe retornar 404
    [Fact]
    public async Task DELETE_DeleteUser_WithNonExistentId_ShouldReturn404()
    {
        // Arrange: ID que no existe
        var nonExistentId = 999;

        // Act: Intentar eliminar usuario inexistente
        var response = await _client.DeleteAsync($"/api/users/{nonExistentId}");

        // Assert: Verificar error 404
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Test - GET /api/users/search con criterios válidos debe retornar 200 y lista de usuarios
    [Fact]
    public async Task GET_SearchUsers_WithValidCriteria_ShouldReturn200AndUserList()
    {
        // Arrange: Crear usuarios de prueba
        var user1 = new UserEntity
        {
            Name = "Juan Pérez",
            Email = "juan@test.com",
            CreationDate = DateTime.UtcNow
        };
        var user2 = new UserEntity
        {
            Name = "Juan Carlos",
            Email = "juan.carlos@test.com",
            CreationDate = DateTime.UtcNow
        };
        var user3 = new UserEntity
        {
            Name = "María García",
            Email = "maria@test.com",
            CreationDate = DateTime.UtcNow
        };

        _dbContext.Users.AddRange(user1, user2, user3);
        await _dbContext.SaveChangesAsync();

        // Act: Buscar usuarios por nombre
        var response = await _client.GetAsync("/api/users?name=Juan");

        // Assert: Verificar búsqueda exitosa
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var searchResults = await GetResponseContent<List<UserResponse>>(response);
        searchResults.Should().NotBeNull();
        searchResults!.Should().HaveCount(2);
        searchResults.Should().OnlyContain(u => u.Name.Contains("Juan"));
    }

    // Test - GET /api/users/search sin criterios debe retornar 400
    [Fact]
    public async Task GET_SearchUsers_WithoutCriteria_ShouldReturn400()
    {
        // Act: Buscar sin criterios
        var response = await _client.GetAsync("/api/users");

        // Assert: Verificar error de validación
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}