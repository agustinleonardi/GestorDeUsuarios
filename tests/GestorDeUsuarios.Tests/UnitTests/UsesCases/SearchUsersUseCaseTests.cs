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

public class SearchUsersUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly SearchUsersUseCase _useCase;

    public SearchUsersUseCaseTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _useCase = new SearchUsersUseCase(_userRepositoryMock.Object, _mapperMock.Object);
    }

    // Test - Buscar usuarios con criterios válidos debe retornar lista de UserResponse
    [Fact]
    public async Task ExecuteAsync_WithValidSearchCriteria_ShouldReturnUserResponseList()
    {
        // Arrange: Configurar búsqueda con criterios válidos
        var searchRequest = TestDataBuilder.CreateSearchRequest(name: "Juan");
        var foundUsers = TestDataBuilder.CreateUserList();
        var expectedResponses = new List<UserResponse>
        {
            TestDataBuilder.CreateValidUserResponse("Juan Pérez", "juan@example.com"),
            TestDataBuilder.CreateValidUserResponse("Juan Carlos", "juan.carlos@example.com")
        };

        _userRepositoryMock
            .Setup(x => x.SearchAsync(searchRequest.Name, searchRequest.Province, searchRequest.City))
            .ReturnsAsync(foundUsers);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<UserResponse>>(foundUsers))
            .Returns(expectedResponses);

        // Act: Ejecutar búsqueda
        var result = await _useCase.ExecuteAsync(searchRequest);

        // Assert: Verificar que se retorna la lista mapeada
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResponses);
        _userRepositoryMock.Verify(x => x.SearchAsync(searchRequest.Name, searchRequest.Province, searchRequest.City), Times.Once);
        _mapperMock.Verify(x => x.Map<IEnumerable<UserResponse>>(foundUsers), Times.Once);
    }

    // Test - Buscar con filtro por nombre debe llamar repositorio con parámetros correctos
    [Fact]
    public async Task ExecuteAsync_WithNameFilter_ShouldCallRepositoryWithCorrectParameters()
    {
        // Arrange: Configurar búsqueda solo por nombre
        var searchRequest = TestDataBuilder.CreateSearchRequest(name: "María");
        var foundUsers = new List<User> { TestDataBuilder.CreateValidUser("María García", "maria@example.com") };
        var expectedResponses = new List<UserResponse> { TestDataBuilder.CreateValidUserResponse("María García", "maria@example.com") };

        _userRepositoryMock
            .Setup(x => x.SearchAsync(searchRequest.Name, searchRequest.Province, searchRequest.City))
            .ReturnsAsync(foundUsers);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<UserResponse>>(foundUsers))
            .Returns(expectedResponses);

        // Act: Ejecutar búsqueda por nombre
        var result = await _useCase.ExecuteAsync(searchRequest);

        // Assert: Verificar llamada con parámetros específicos
        result.Should().NotBeNull();
        _userRepositoryMock.Verify(x => x.SearchAsync("María", null, null), Times.Once);
    }

    // Test - Buscar con filtro por provincia debe llamar repositorio correctamente
    [Fact]
    public async Task ExecuteAsync_WithProvinceFilter_ShouldCallRepositoryWithCorrectParameters()
    {
        // Arrange: Configurar búsqueda solo por provincia
        var searchRequest = TestDataBuilder.CreateSearchRequest(province: "Buenos Aires");
        var foundUsers = TestDataBuilder.CreateUserList();
        var expectedResponses = new List<UserResponse> { TestDataBuilder.CreateValidUserResponse() };

        _userRepositoryMock
            .Setup(x => x.SearchAsync(searchRequest.Name, searchRequest.Province, searchRequest.City))
            .ReturnsAsync(foundUsers);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<UserResponse>>(foundUsers))
            .Returns(expectedResponses);

        // Act: Ejecutar búsqueda por provincia
        var result = await _useCase.ExecuteAsync(searchRequest);

        // Assert: Verificar llamada con provincia específica
        result.Should().NotBeNull();
        _userRepositoryMock.Verify(x => x.SearchAsync(null, "Buenos Aires", null), Times.Once);
    }

    // Test - Buscar con filtro por ciudad debe llamar repositorio correctamente
    [Fact]
    public async Task ExecuteAsync_WithCityFilter_ShouldCallRepositoryWithCorrectParameters()
    {
        // Arrange: Configurar búsqueda solo por ciudad
        var searchRequest = TestDataBuilder.CreateSearchRequest(city: "CABA");
        var foundUsers = TestDataBuilder.CreateUserList();
        var expectedResponses = new List<UserResponse> { TestDataBuilder.CreateValidUserResponse() };

        _userRepositoryMock
            .Setup(x => x.SearchAsync(searchRequest.Name, searchRequest.Province, searchRequest.City))
            .ReturnsAsync(foundUsers);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<UserResponse>>(foundUsers))
            .Returns(expectedResponses);

        // Act: Ejecutar búsqueda por ciudad
        var result = await _useCase.ExecuteAsync(searchRequest);

        // Assert: Verificar llamada con ciudad específica
        result.Should().NotBeNull();
        _userRepositoryMock.Verify(x => x.SearchAsync(null, null, "CABA"), Times.Once);
    }

    // Test - Buscar con múltiples filtros debe llamar repositorio con todos los parámetros
    [Fact]
    public async Task ExecuteAsync_WithMultipleFilters_ShouldCallRepositoryWithAllParameters()
    {
        // Arrange: Configurar búsqueda con múltiples criterios
        var searchRequest = TestDataBuilder.CreateSearchRequest("Juan", "CABA", "Buenos Aires");
        var foundUsers = new List<User> { TestDataBuilder.CreateValidUser() };
        var expectedResponses = new List<UserResponse> { TestDataBuilder.CreateValidUserResponse() };

        _userRepositoryMock
            .Setup(x => x.SearchAsync(searchRequest.Name, searchRequest.Province, searchRequest.City))
            .ReturnsAsync(foundUsers);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<UserResponse>>(foundUsers))
            .Returns(expectedResponses);

        // Act: Ejecutar búsqueda con múltiples filtros
        var result = await _useCase.ExecuteAsync(searchRequest);

        // Assert: Verificar llamada con todos los parámetros
        result.Should().NotBeNull();
        _userRepositoryMock.Verify(x => x.SearchAsync("Juan", "Buenos Aires", "CABA"), Times.Once);
    }

    // Test - Buscar sin criterios debe lanzar InvalidSearchCriteriaException
    [Fact]
    public async Task ExecuteAsync_WithEmptySearchCriteria_ShouldThrowInvalidSearchCriteriaException()
    {
        // Arrange: Configurar búsqueda sin criterios (todos null/empty)
        var emptySearchRequest = TestDataBuilder.CreateSearchRequest(); // Todos null por defecto

        // Act & Assert: Verificar que se lanza la excepción correcta
        var exception = await Assert.ThrowsAsync<InvalidSearchCriteriaException>(
            () => _useCase.ExecuteAsync(emptySearchRequest));

        // Assert: Verificar que no se llamó al repositorio
        _userRepositoryMock.Verify(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mapperMock.Verify(x => x.Map<IEnumerable<UserResponse>>(It.IsAny<IEnumerable<User>>()), Times.Never);
    }

    // Test - Buscar con strings vacíos debe lanzar InvalidSearchCriteriaException
    [Fact]
    public async Task ExecuteAsync_WithWhitespaceSearchCriteria_ShouldThrowInvalidSearchCriteriaException()
    {
        // Arrange: Configurar búsqueda con strings vacíos/espacios
        var whitespaceSearchRequest = TestDataBuilder.CreateSearchRequest("   ", "  ", "    ");

        // Act & Assert: Verificar que se lanza la excepción correcta
        var exception = await Assert.ThrowsAsync<InvalidSearchCriteriaException>(
            () => _useCase.ExecuteAsync(whitespaceSearchRequest));

        // Assert: Verificar que no se llamó al repositorio
        _userRepositoryMock.Verify(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    // Test - Buscar cuando no se encuentran usuarios debe retornar lista vacía
    [Fact]
    public async Task ExecuteAsync_WhenNoUsersFound_ShouldReturnEmptyList()
    {
        // Arrange: Configurar búsqueda que no encuentra usuarios
        var searchRequest = TestDataBuilder.CreateSearchRequest(name: "UsuarioInexistente");
        var emptyUserList = new List<User>();
        var emptyResponseList = new List<UserResponse>();

        _userRepositoryMock
            .Setup(x => x.SearchAsync(searchRequest.Name, searchRequest.Province, searchRequest.City))
            .ReturnsAsync(emptyUserList);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<UserResponse>>(emptyUserList))
            .Returns(emptyResponseList);

        // Act: Ejecutar búsqueda sin resultados
        var result = await _useCase.ExecuteAsync(searchRequest);

        // Assert: Verificar que se retorna lista vacía
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _userRepositoryMock.Verify(x => x.SearchAsync(searchRequest.Name, searchRequest.Province, searchRequest.City), Times.Once);
        _mapperMock.Verify(x => x.Map<IEnumerable<UserResponse>>(emptyUserList), Times.Once);
    }

    // Test - Error en repositorio debe propagar excepción
    [Fact]
    public async Task ExecuteAsync_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange: Configurar repositorio para lanzar excepción
        var searchRequest = TestDataBuilder.CreateSearchRequest(name: "Juan");
        var expectedException = new InvalidOperationException("Error de base de datos");

        _userRepositoryMock
            .Setup(x => x.SearchAsync(searchRequest.Name, searchRequest.Province, searchRequest.City))
            .ThrowsAsync(expectedException);

        // Act & Assert: Verificar que se propaga la excepción original
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(searchRequest));

        // Assert: Verificar que es la misma excepción
        exception.Should().Be(expectedException);
        _userRepositoryMock.Verify(x => x.SearchAsync(searchRequest.Name, searchRequest.Province, searchRequest.City), Times.Once);
        _mapperMock.Verify(x => x.Map<IEnumerable<UserResponse>>(It.IsAny<IEnumerable<User>>()), Times.Never);
    }
}