using FluentAssertions;
using FluentValidation.TestHelper;
using GestorDeUsuarios.Application.Models.Requests;
using GestorDeUsuarios.Application.Validators;
using Xunit;

namespace GestorDeUsuarios.Tests.UnitTests.Validators;

public class SearchUsersRequestValidatorTests
{
    private readonly SearchUsersRequestValidator _validator;

    public SearchUsersRequestValidatorTests()
    {
        _validator = new SearchUsersRequestValidator();
    }

    // Test - Request de búsqueda con datos válidos debe pasar validación
    [Fact]
    public void Validate_WithValidSearchRequest_ShouldNotHaveErrors()
    {
        // Arrange: Crear request de búsqueda con todos los filtros válidos
        var request = new SearchUsersRequest("Juan", "Buenos Aires", "CABA");

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Test - Request de búsqueda con solo nombre debe pasar validación
    [Fact]
    public void Validate_WithOnlyName_ShouldNotHaveErrors()
    {
        // Arrange: Crear request solo con filtro por nombre
        var request = new SearchUsersRequest("María", null, null);

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Test - Request de búsqueda con solo provincia debe pasar validación
    [Fact]
    public void Validate_WithOnlyProvince_ShouldNotHaveErrors()
    {
        // Arrange: Crear request solo con filtro por provincia
        var request = new SearchUsersRequest(null, "Córdoba", null);

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Test - Request de búsqueda con solo ciudad debe pasar validación
    [Fact]
    public void Validate_WithOnlyCity_ShouldNotHaveErrors()
    {
        // Arrange: Crear request solo con filtro por ciudad
        var request = new SearchUsersRequest(null, null, "Rosario");

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Test - Request de búsqueda sin filtros debe pasar validación (validación de negocio se hace en Use Case)
    [Fact]
    public void Validate_WithNoFilters_ShouldNotHaveErrors()
    {
        // Arrange: Crear request sin filtros (todos null)
        var request = new SearchUsersRequest(null, null, null);

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores (validación de negocio se hace en Use Case)
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Test - Request con filtros vacíos debe pasar validación
    [Fact]
    public void Validate_WithEmptyFilters_ShouldNotHaveErrors()
    {
        // Arrange: Crear request con strings vacíos
        var request = new SearchUsersRequest("", "", "");

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores (strings vacíos no activan las validaciones condicionales)
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Test - Request con filtros de solo espacios debe pasar validación
    [Fact]
    public void Validate_WithWhitespaceFilters_ShouldNotHaveErrors()
    {
        // Arrange: Crear request con solo espacios en blanco
        var request = new SearchUsersRequest("   ", "   ", "   ");

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores (espacios no activan las validaciones condicionales)
        result.ShouldNotHaveAnyValidationErrors();
    }

    #region Name Filter Validation Tests

    // Test - Nombre muy largo debe fallar validación
    [Fact]
    public void Validate_WithTooLongName_ShouldHaveError()
    {
        // Arrange: Crear nombre de búsqueda de 101 caracteres (más del límite de 100)
        var longName = new string('N', 101);
        var request = new SearchUsersRequest(longName, null, null);

        // Act & Assert: Validar y verificar error de longitud en nombre
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    // Test - Nombre en el límite debe pasar validación
    [Fact]
    public void Validate_WithMaxLengthName_ShouldNotHaveError()
    {
        // Arrange: Crear nombre de exactamente 100 caracteres (en el límite)
        var maxLengthName = new string('A', 100);
        var request = new SearchUsersRequest(maxLengthName, null, null);

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region Province Filter Validation Tests

    // Test - Provincia muy larga debe fallar validación
    [Fact]
    public void Validate_WithTooLongProvince_ShouldHaveError()
    {
        // Arrange: Crear provincia de búsqueda de 101 caracteres (más del límite de 100)
        var longProvince = new string('P', 101);
        var request = new SearchUsersRequest(null, longProvince, null);

        // Act & Assert: Validar y verificar error de longitud en provincia
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Province);
    }

    // Test - Provincia en el límite debe pasar validación
    [Fact]
    public void Validate_WithMaxLengthProvince_ShouldNotHaveError()
    {
        // Arrange: Crear provincia de exactamente 100 caracteres (en el límite)
        var maxLengthProvince = new string('B', 100);
        var request = new SearchUsersRequest(null, maxLengthProvince, null);

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveValidationErrorFor(x => x.Province);
    }

    #endregion

    #region City Filter Validation Tests

    // Test - Ciudad muy larga debe fallar validación
    [Fact]
    public void Validate_WithTooLongCity_ShouldHaveError()
    {
        // Arrange: Crear ciudad de búsqueda de 101 caracteres (más del límite de 100)
        var longCity = new string('C', 101);
        var request = new SearchUsersRequest(null, null, longCity);

        // Act & Assert: Validar y verificar error de longitud en ciudad
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    // Test - Ciudad en el límite debe pasar validación
    [Fact]
    public void Validate_WithMaxLengthCity_ShouldNotHaveError()
    {
        // Arrange: Crear ciudad de exactamente 100 caracteres (en el límite)
        var maxLengthCity = new string('R', 100);
        var request = new SearchUsersRequest(null, null, maxLengthCity);

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveValidationErrorFor(x => x.City);
    }

    #endregion

    #region Multiple Filters Combination Tests

    // Test - Múltiples filtros válidos deben pasar validación
    [Fact]
    public void Validate_WithMultipleValidFilters_ShouldNotHaveErrors()
    {
        // Arrange: Crear request con múltiples filtros válidos
        var request = new SearchUsersRequest("Carlos María", "Buenos Aires", "Capital Federal");

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Test - Combinación de filtros válidos e inválidos debe tener solo errores en los inválidos
    [Fact]
    public void Validate_WithMixedValidAndInvalidFilters_ShouldHaveErrorsOnlyForInvalid()
    {
        // Arrange: Crear request con nombre válido pero ciudad muy larga
        var validName = "Ana";
        var invalidCity = new string('X', 101); // Ciudad muy larga
        var request = new SearchUsersRequest(validName, null, invalidCity);

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: Solo debe tener error en la ciudad, no en el nombre
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    #endregion
}