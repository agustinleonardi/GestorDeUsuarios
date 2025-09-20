using AutoMapper;
using FluentAssertions;
using GestorDeUsuarios.Application.Exceptions;
using GestorDeUsuarios.Application.UsesCases;
using GestorDeUsuarios.Domain.Abstractions.Repositories;
using GestorDeUsuarios.Domain.Models;
using GestorDeUsuarios.Tests.Helpers;
using Moq;
using Xunit;

namespace GestorDeUsuarios.Tests.UnitTests.UsesCases;

public class DeleteUserUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DeleteUserUseCase _useCase;

    public DeleteUserUseCaseTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _useCase = new DeleteUserUseCase(_userRepositoryMock.Object, _mapperMock.Object);
    }

    // Test - Eliminar usuario existente debe ejecutar eliminación en repositorio
    [Fact]
    public async Task ExecuteAsync_WithExistingUser_ShouldDeleteUserSuccessfully()
    {
        // Arrange: Configurar usuario existente para eliminar
        var userId = 1;
        var existingUser = TestDataBuilder.CreateValidUserWithId(userId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(x => x.DeleteAsync(userId))
            .Returns(Task.CompletedTask);

        // Act: Ejecutar eliminación
        await _useCase.ExecuteAsync(userId);

        // Assert: Verificar que se buscó y eliminó el usuario
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(x => x.DeleteAsync(userId), Times.Once);
    }

    // Test - Eliminar usuario inexistente debe lanzar UserNotFoundApplicationException
    [Fact]
    public async Task ExecuteAsync_WithNonExistentUser_ShouldThrowUserNotFoundApplicationException()
    {
        // Arrange: Configurar usuario que no existe
        var userId = 999;

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act & Assert: Verificar que se lanza la excepción correcta
        var exception = await Assert.ThrowsAsync<UserNotFoundApplicationException>(
            () => _useCase.ExecuteAsync(userId));

        // Assert: Verificar mensaje de error y que no se intentó eliminar
        exception.Message.Should().Contain(userId.ToString());
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    // Test - Error en repositorio al buscar usuario debe propagar excepción
    [Fact]
    public async Task ExecuteAsync_WhenRepositoryThrowsOnGet_ShouldPropagateException()
    {
        // Arrange: Configurar repositorio para lanzar excepción al buscar
        var userId = 1;
        var expectedException = new InvalidOperationException("Error de base de datos");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ThrowsAsync(expectedException);

        // Act & Assert: Verificar que se propaga la excepción original
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(userId));

        // Assert: Verificar que es la misma excepción y no se intentó eliminar
        exception.Should().Be(expectedException);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    // Test - Error en repositorio al eliminar usuario debe propagar excepción
    [Fact]
    public async Task ExecuteAsync_WhenRepositoryThrowsOnDelete_ShouldPropagateException()
    {
        // Arrange: Configurar usuario existente pero error al eliminar
        var userId = 1;
        var existingUser = TestDataBuilder.CreateValidUserWithId(userId);
        var expectedException = new InvalidOperationException("Error al eliminar usuario");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(x => x.DeleteAsync(userId))
            .ThrowsAsync(expectedException);

        // Act & Assert: Verificar que se propaga la excepción del delete
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(userId));

        // Assert: Verificar que es la excepción correcta
        exception.Should().Be(expectedException);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(x => x.DeleteAsync(userId), Times.Once);
    }
}