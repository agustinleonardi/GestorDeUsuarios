using AutoMapper;
using FluentAssertions;
using GestorDeUsuarios.Application.Exceptions;
using GestorDeUsuarios.Application.Models.Requests;
using GestorDeUsuarios.Application.Models.Responses;
using GestorDeUsuarios.Application.UsesCases;
using GestorDeUsuarios.Domain.Abstractions.Repositories;
using GestorDeUsuarios.Domain.Models;
using GestorDeUsuarios.Tests.Helpers;
using Moq;
using Xunit;

namespace GestorDeUsuarios.Tests.UnitTests.UsesCases;

public class UpdateUserUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAddressRepository> _addressRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateUserUseCase _useCase;

    public UpdateUserUseCaseTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _addressRepositoryMock = new Mock<IAddressRepository>();
        _mapperMock = new Mock<IMapper>();
        _useCase = new UpdateUserUseCase(_userRepositoryMock.Object, _addressRepositoryMock.Object, _mapperMock.Object);
    }

    // Test - Actualizar usuario existente con datos válidos debe retornar UserResponse
    [Fact]
    public async Task ExecuteAsync_WithValidUserAndRequest_ShouldReturnUpdatedUserResponse()
    {
        // Arrange: Configurar usuario existente y datos de actualización
        var userId = 1;
        var existingUser = TestDataBuilder.CreateValidUserWithId(userId);
        var updateRequest = TestDataBuilder.CreateValidUpdateUserRequest();
        var expectedResponse = TestDataBuilder.CreateValidUserResponse();

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(updateRequest.Email))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(x => x.Map<UserResponse>(It.IsAny<User>()))
            .Returns(expectedResponse);

        // Act: Ejecutar actualización
        var result = await _useCase.ExecuteAsync(userId, updateRequest);

        // Assert: Verificar que se actualizó correctamente
        result.Should().NotBeNull();
        result.Should().Be(expectedResponse);

        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(x => x.GetByEmailAsync(updateRequest.Email), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        _mapperMock.Verify(x => x.Map<UserResponse>(It.IsAny<User>()), Times.Once);
    }

    // Test - Actualizar usuario inexistente debe lanzar UserNotFoundApplicationException
    [Fact]
    public async Task ExecuteAsync_WithNonExistentUser_ShouldThrowUserNotFoundApplicationException()
    {
        // Arrange: Configurar ID de usuario que no existe en el repositorio
        var userId = 999;
        var updateRequest = TestDataBuilder.CreateValidUpdateUserRequest();

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act & Assert: Verificar que se lanza la excepción correcta
        var exception = await Assert.ThrowsAsync<UserNotFoundApplicationException>(
            () => _useCase.ExecuteAsync(userId, updateRequest));

        // Assert: Verificar mensaje de error y que no se intentó actualizar
        exception.Message.Should().Contain(userId.ToString());
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    // Test - Actualizar usuario con email ya existente en otro usuario debe lanzar UserAlreadyExistsException
    [Fact]
    public async Task ExecuteAsync_WithExistingEmail_ShouldThrowUserAlreadyExistsException()
    {
        // Arrange: Configurar usuario a actualizar y otro usuario que ya tiene el email deseado
        var userId = 1;
        var existingUser = TestDataBuilder.CreateValidUserWithId(userId);
        var updateRequest = TestDataBuilder.CreateValidUpdateUserRequest();
        var userWithSameEmail = TestDataBuilder.CreateValidUserWithId(2); // Diferente ID, mismo email

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(updateRequest.Email))
            .ReturnsAsync(userWithSameEmail);

        // Act & Assert: Verificar que se lanza la excepción de email duplicado
        var exception = await Assert.ThrowsAsync<UserAlreadyExistsException>(
            () => _useCase.ExecuteAsync(userId, updateRequest));

        // Assert: Verificar mensaje de error y que no se actualizó el usuario
        exception.Message.Should().Contain(updateRequest.Email);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(x => x.GetByEmailAsync(updateRequest.Email), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    // Test - Actualizar usuario manteniendo su propio email debe permitir la actualización
    [Fact]
    public async Task ExecuteAsync_WithSameEmailForSameUser_ShouldAllowUpdate()
    {
        // Arrange: Configurar usuario que mantiene su mismo email (caso válido)
        var userId = 1;
        var existingUser = TestDataBuilder.CreateValidUserWithId(userId);
        var updateRequest = TestDataBuilder.CreateValidUpdateUserRequest();
        updateRequest = updateRequest with { Email = existingUser.Email }; // Mismo email del usuario

        var expectedResponse = TestDataBuilder.CreateValidUserResponse();

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(updateRequest.Email))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(x => x.Map<UserResponse>(It.IsAny<User>()))
            .Returns(expectedResponse);

        // Act: Ejecutar actualización con mismo email
        var result = await _useCase.ExecuteAsync(userId, updateRequest);

        // Assert: Verificar que se permite la actualización cuando es el mismo usuario
        result.Should().NotBeNull();
        result.Should().Be(expectedResponse);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    // Test - Actualizar usuario con dirección null debe eliminar dirección existente
    [Fact]
    public async Task ExecuteAsync_WithNullAddress_ShouldRemoveExistingAddress()
    {
        // Arrange: Configurar usuario que tiene dirección y request sin dirección
        var userId = 1;
        var existingUser = TestDataBuilder.CreateValidUserWithId(userId); // Usuario con ID
        var address = TestDataBuilder.CreateValidAddress(userId); // Crear dirección para este usuario
        existingUser.AssignAddress(address); // Asignar dirección manualmente

        var updateRequest = TestDataBuilder.CreateValidUpdateUserRequest(); // Sin dirección (null)
        var expectedResponse = TestDataBuilder.CreateValidUserResponse();

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(updateRequest.Email))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(x => x.Map<UserResponse>(It.IsAny<User>()))
            .Returns(expectedResponse);

        // Act: Ejecutar actualización sin dirección
        var result = await _useCase.ExecuteAsync(userId, updateRequest);

        // Assert: Verificar que se elimina la dirección existente
        result.Should().NotBeNull();
        result.Should().Be(expectedResponse);
        existingUser.Address.Should().BeNull(); // La dirección debe haber sido eliminada
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    // Test - Actualizar usuario sin dirección agregando nueva dirección debe crear dirección
    [Fact]
    public async Task ExecuteAsync_WithNewAddress_ShouldCreateNewAddress()
    {
        // Arrange: Configurar usuario SIN dirección y request CON dirección nueva
        var userId = 1;
        var existingUser = TestDataBuilder.CreateValidUserWithId(userId); // Sin dirección
        var updateRequest = TestDataBuilder.CreateValidUpdateUserRequestWithAddress(); // Con nueva dirección
        var newAddress = TestDataBuilder.CreateValidAddressWithId(5, userId);
        var expectedResponse = TestDataBuilder.CreateValidUserResponse();

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(updateRequest.Email))
            .ReturnsAsync((User?)null);

        _addressRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Address>()))
            .ReturnsAsync(newAddress);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(x => x.Map<UserResponse>(It.IsAny<User>()))
            .Returns(expectedResponse);

        // Act: Ejecutar actualización con nueva dirección
        var result = await _useCase.ExecuteAsync(userId, updateRequest);

        // Assert: Verificar que se crea nueva dirección y se asigna al usuario
        result.Should().NotBeNull();
        result.Should().Be(expectedResponse);
        _addressRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Address>()), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    // Test - Actualizar usuario que ya tiene dirección debe modificar datos de dirección existente
    [Fact]
    public async Task ExecuteAsync_WithExistingAddress_ShouldUpdateAddressFields()
    {
        // Arrange: Configurar usuario CON dirección y request para actualizar esa dirección
        var userId = 1;
        var existingUser = TestDataBuilder.CreateValidUserWithId(userId); // Usuario con ID
        var address = TestDataBuilder.CreateValidAddress(userId); // Crear dirección para este usuario
        existingUser.AssignAddress(address); // Asignar dirección manualmente

        var updateRequest = TestDataBuilder.CreateValidUpdateUserRequestWithAddress(); // Nueva datos de dirección
        var expectedResponse = TestDataBuilder.CreateValidUserResponse();

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(updateRequest.Email))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(x => x.Map<UserResponse>(It.IsAny<User>()))
            .Returns(expectedResponse);

        // Act: Ejecutar actualización de dirección existente
        var result = await _useCase.ExecuteAsync(userId, updateRequest);

        // Assert: Verificar que se actualiza la dirección existente (no se crea nueva)
        result.Should().NotBeNull();
        result.Should().Be(expectedResponse);
        existingUser.Address.Should().NotBeNull(); // Sigue teniendo dirección
        _addressRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Address>()), Times.Never); // No se crea nueva
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    // Test - Actualizar usuario con ID inválido debe lanzar UserNotFoundApplicationException
    [Fact]
    public async Task ExecuteAsync_WithInvalidUserId_ShouldThrowUserNotFoundApplicationException()
    {
        // Arrange: Configurar ID de usuario inválido que no existe en el repositorio
        var invalidUserId = 0;
        var updateRequest = TestDataBuilder.CreateValidUpdateUserRequest();

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(invalidUserId))
            .ReturnsAsync((User?)null);

        // Act & Assert: Verificar que se lanza excepción porque el usuario no existe
        var exception = await Assert.ThrowsAsync<UserNotFoundApplicationException>(
            () => _useCase.ExecuteAsync(invalidUserId, updateRequest));

        // Assert: Verificar mensaje de error y que se buscó el usuario
        exception.Message.Should().Contain(invalidUserId.ToString());
        _userRepositoryMock.Verify(x => x.GetByIdAsync(invalidUserId), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    // Test - Actualizar usuario con request nulo debe lanzar UserNotFoundApplicationException (porque busca el usuario primero)
    [Fact]
    public async Task ExecuteAsync_WithNullRequest_ShouldThrowUserNotFoundApplicationException()
    {
        // Arrange: Configurar request nulo y usuario que no existe
        var userId = 1;
        UpdateUserRequest? nullRequest = null;

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act & Assert: Verificar que se lanza excepción porque busca usuario primero
        var exception = await Assert.ThrowsAsync<UserNotFoundApplicationException>(
            () => _useCase.ExecuteAsync(userId, nullRequest!));

        // Assert: Verificar que se intentó buscar el usuario
        exception.Message.Should().Contain(userId.ToString());
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    // Test - Error en repositorio de usuarios debe propagar la excepción
    [Fact]
    public async Task ExecuteAsync_WhenUserRepositoryThrows_ShouldPropagateException()
    {
        // Arrange: Configurar repositorio para lanzar excepción
        var userId = 1;
        var updateRequest = TestDataBuilder.CreateValidUpdateUserRequest();
        var expectedException = new InvalidOperationException("Error de base de datos");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ThrowsAsync(expectedException);

        // Act & Assert: Verificar que se propaga la excepción original
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(userId, updateRequest));

        // Assert: Verificar que es la misma excepción y no se intentó actualizar
        exception.Should().Be(expectedException);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    // Test - Error en repositorio de direcciones debe propagar la excepción
    [Fact]
    public async Task ExecuteAsync_WhenAddressRepositoryFails_ShouldThrowException()
    {
        // Arrange: Configurar usuario sin dirección y error en AddressRepository
        var userId = 1;
        var existingUser = TestDataBuilder.CreateValidUserWithId(userId);
        var updateRequest = TestDataBuilder.CreateValidUpdateUserRequestWithAddress();
        var expectedException = new InvalidOperationException("Error al crear dirección");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(updateRequest.Email))
            .ReturnsAsync((User?)null);

        _addressRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Address>()))
            .ThrowsAsync(expectedException);

        // Act & Assert: Verificar que se propaga la excepción del repositorio de direcciones
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(userId, updateRequest));

        // Assert: Verificar que es la excepción correcta y no se actualizó el usuario
        exception.Should().Be(expectedException);
        _addressRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Address>()), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
}