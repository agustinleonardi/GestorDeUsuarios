using AutoMapper;
using GestorDeUsuarios.Application.Abstractions.UsesCases;
using GestorDeUsuarios.Application.Exceptions;
using GestorDeUsuarios.Application.Models.Responses;
using GestorDeUsuarios.Domain.Abstractions.Repositories;
using GestorDeUsuarios.Domain.Models;
using GestorDeUsuarios.Tests.Helpers;

namespace GestorDeUsuarios.Tests.UnitTests.UsesCases;

public class GetUserByIdUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetUserByIdUseCase _useCase;

    public GetUserByIdUseCaseTests()
    {
        // Inicializar mocks y clase a testear
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _useCase = new GetUserByIdUseCase(_userRepositoryMock.Object, _mapperMock.Object);
    }

    // Test: Obtener usuario existente por ID debe retornar UserResponse
    [Fact]
    public async Task ExecuteAsync_WithExistingUserId_ShouldReturnUserResponse()
    {
        // Arrange: Configurar usuario existente y comportamiento de mocks
        var userId = 1;
        var existingUser = TestDataBuilder.CreateValidUserWithId(userId);
        var expectedResponse = TestDataBuilder.CreateValidUserResponse(existingUser.Name, existingUser.Email);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _mapperMock
            .Setup(x => x.Map<UserResponse>(existingUser))
            .Returns(expectedResponse);

        // Act: Ejecutar búsqueda por ID
        var result = await _useCase.ExecuteAsync(userId);

        // Assert: Verificar que retorna el usuario correcto
        result.Should().NotBeNull();
        result.Name.Should().Be(expectedResponse.Name);
        result.Email.Should().Be(expectedResponse.Email);

        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _mapperMock.Verify(x => x.Map<UserResponse>(existingUser), Times.Once);
    }

    // Test: Buscar usuario inexistente debe lanzar UserNotFoundApplicationException
    [Fact]
    public async Task ExecuteAsync_WithNonExistingUserId_ShouldThrowUserNotFoundApplicationException()
    {
        // Arrange: Simular que el usuario no existe (retorna null)
        var userId = 999;

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act & Assert: Ejecutar y verificar que lance la excepción correcta
        var exception = await Assert.ThrowsAsync<UserNotFoundApplicationException>(
            () => _useCase.ExecuteAsync(userId));

        exception.Message.Should().Contain(userId.ToString());
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _mapperMock.Verify(x => x.Map<UserResponse>(It.IsAny<User>()), Times.Never);
    }

    // Test: Buscar con IDs inválidos (0, negativos) debe lanzar UserNotFoundApplicationException
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task ExecuteAsync_WithInvalidUserId_ShouldThrowUserNotFoundApplicationException(int invalidUserId)
    {
        // Arrange: Configurar que no encuentra el usuario con ID inválido
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(invalidUserId))
            .ReturnsAsync((User?)null);

        // Act & Assert: Ejecutar y verificar que falle por ID inválido
        var exception = await Assert.ThrowsAsync<UserNotFoundApplicationException>(
            () => _useCase.ExecuteAsync(invalidUserId));

        exception.Message.Should().Contain(invalidUserId.ToString());
        _userRepositoryMock.Verify(x => x.GetByIdAsync(invalidUserId), Times.Once);
    }

    // Test: Error en repositorio debe propagar la excepción
    [Fact]
    public async Task ExecuteAsync_WhenRepositoryFails_ShouldPropagateException()
    {
        // Arrange: Configurar repositorio para que falle
        var userId = 1;
        var expectedException = new InvalidOperationException("Database connection failed");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ThrowsAsync(expectedException);

        // Act & Assert: Ejecutar y verificar que se propague la excepción
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(userId));

        exception.Should().Be(expectedException);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _mapperMock.Verify(x => x.Map<UserResponse>(It.IsAny<User>()), Times.Never);
    }
}