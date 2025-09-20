using AutoMapper;
using GestorDeUsuarios.Application.UsesCases;
using GestorDeUsuarios.Application.Exceptions;
using GestorDeUsuarios.Domain.Abstractions.Repositories;
using GestorDeUsuarios.Domain.Exceptions;
using GestorDeUsuarios.Domain.Models;
using GestorDeUsuarios.Application.Models.Responses;
using GestorDeUsuarios.Tests.Helpers;

namespace GestorDeUsuarios.Tests.UnitTests.UsesCases;

public class CreateUserUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAddressRepository> _addressRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateUserUseCase _useCase;

    public CreateUserUseCaseTests()
    {
        // Inicializar mocks y clase a testear
        _userRepositoryMock = new Mock<IUserRepository>();
        _addressRepositoryMock = new Mock<IAddressRepository>();
        _mapperMock = new Mock<IMapper>();
        _useCase = new CreateUserUseCase(_userRepositoryMock.Object, _mapperMock.Object, _addressRepositoryMock.Object);
    }

    // Test: Crear usuario con datos válidos debe retornar UserResponse
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldReturnUserResponse()
    {
        // Arrange: Configurar datos de prueba y comportamiento de mocks
        var request = TestDataBuilder.CreateValidUserRequest();
        var expectedUser = TestDataBuilder.CreateValidUser();
        var expectedResponse = TestDataBuilder.CreateValidUserResponse(request.Name, request.Email);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(expectedUser);

        _mapperMock
            .Setup(x => x.Map<UserResponse>(expectedUser))
            .Returns(expectedResponse);

        // Act: Ejecutar el método a testear
        var result = await _useCase.ExecuteAsync(request);

        // Assert: Verificar resultado y llamadas a dependencias
        result.Should().NotBeNull();
        result.Name.Should().Be(expectedResponse.Name);
        result.Email.Should().Be(expectedResponse.Email);

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(request.Email), Times.Once);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        _mapperMock.Verify(x => x.Map<UserResponse>(expectedUser), Times.Once);
    }

    // Test: Crear usuario con email existente debe lanzar UserAlreadyExistsException
    [Fact]
    public async Task ExecuteAsync_WithExistingEmail_ShouldThrowUserAlreadyExistsException()
    {
        // Arrange: Simular que ya existe un usuario con el email
        var request = TestDataBuilder.CreateValidUserRequest();
        var existingUser = TestDataBuilder.CreateValidUser();

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(existingUser);

        // Act & Assert: Ejecutar y verificar que lance la excepción esperada
        var exception = await Assert.ThrowsAsync<UserAlreadyExistsException>(
            () => _useCase.ExecuteAsync(request));

        exception.Message.Should().Contain(request.Email);
        _userRepositoryMock.Verify(x => x.GetByEmailAsync(request.Email), Times.Once);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
    }

    // Test: Crear usuario con nombre inválido debe lanzar InvalidUserDataException
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task ExecuteAsync_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange: Crear petición con nombre inválido
        var request = TestDataBuilder.CreateValidUserRequest(name: invalidName);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        // Act & Assert: Ejecutar y verificar que falle por el nombre inválido
        var exception = await Assert.ThrowsAsync<InvalidUserDataException>(
            () => _useCase.ExecuteAsync(request));

        exception.Message.Should().Contain("nombre");
    }

    // Test: Crear usuario con email inválido debe lanzar InvalidUserDataException  
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExecuteAsync_WithInvalidEmail_ShouldThrowInvalidUserDataException(string invalidEmail)
    {
        // Arrange: Crear petición con email inválido
        var request = TestDataBuilder.CreateValidUserRequest("Juan Pérez", invalidEmail);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        // Act & Assert: Ejecutar y verificar que falle por el email inválido
        var exception = await Assert.ThrowsAsync<InvalidUserDataException>(
            () => _useCase.ExecuteAsync(request));

        exception.Message.Should().Contain("email");
    }

    // Test: Crear usuario con request null debe lanzar ArgumentNullException
    [Fact]
    public async Task ExecuteAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Act & Assert: Ejecutar con null y verificar excepción
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _useCase.ExecuteAsync(null!));
    }

    // Test: Error en repositorio debe propagar la excepción
    [Fact]
    public async Task ExecuteAsync_WhenRepositoryFails_ShouldPropagateException()
    {
        // Arrange: Configurar repositorio para que falle
        var request = TestDataBuilder.CreateValidUserRequest();
        var expectedException = new InvalidOperationException("Database error");

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ThrowsAsync(expectedException);

        // Act & Assert: Ejecutar y verificar que se propague la excepción
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(request));

        exception.Should().Be(expectedException);
    }
}