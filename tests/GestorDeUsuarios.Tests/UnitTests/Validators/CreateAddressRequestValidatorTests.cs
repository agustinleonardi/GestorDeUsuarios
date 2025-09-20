using FluentAssertions;
using FluentValidation.TestHelper;
using GestorDeUsuarios.Application.Models.Requests;
using GestorDeUsuarios.Application.Validators;
using Xunit;

namespace GestorDeUsuarios.Tests.UnitTests.Validators;

public class CreateAddressRequestValidatorTests
{
    private readonly CreateAddressRequestValidator _validator;

    public CreateAddressRequestValidatorTests()
    {
        _validator = new CreateAddressRequestValidator();
    }

    // Test - Request de dirección con datos válidos debe pasar validación
    [Fact]
    public void Validate_WithValidAddressRequest_ShouldNotHaveErrors()
    {
        // Arrange: Crear request de dirección con todos los datos válidos
        var request = new CreateAddressRequest("Av. Libertador", "1500", "Buenos Aires", "Vicente López");

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveAnyValidationErrors();
    }

    #region Street Validation Tests

    // Test - Calle vacía debe fallar validación
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithEmptyStreet_ShouldHaveError(string invalidStreet)
    {
        // Arrange: Crear request con calle inválida
        var request = new CreateAddressRequest(invalidStreet, "1500", "Buenos Aires", "Vicente López");

        // Act & Assert: Validar y verificar error en calle
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Street)
            .WithErrorMessage("La calle es requerida");
    }

    // Test - Calle muy larga debe fallar validación
    [Fact]
    public void Validate_WithTooLongStreet_ShouldHaveError()
    {
        // Arrange: Crear calle de 151 caracteres (más del límite de 150)
        var longStreet = new string('S', 151);
        var request = new CreateAddressRequest(longStreet, "1500", "Buenos Aires", "Vicente López");

        // Act & Assert: Validar y verificar error de longitud
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Street)
            .WithErrorMessage("La calle no puede tener más de 150 caracteres");
    }

    // Test - Calle en el límite debe pasar validación
    [Fact]
    public void Validate_WithMaxLengthStreet_ShouldNotHaveError()
    {
        // Arrange: Crear calle de exactamente 150 caracteres (en el límite)
        var maxLengthStreet = new string('A', 150);
        var request = new CreateAddressRequest(maxLengthStreet, "1500", "Buenos Aires", "Vicente López");

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveValidationErrorFor(x => x.Street);
    }

    // Test - Calles válidas con diferentes formatos deben pasar validación
    [Theory]
    [InlineData("Av. Corrientes")]
    [InlineData("Calle 123")]
    [InlineData("Pasaje San Martín")]
    [InlineData("Boulevard de los Alisos")]
    [InlineData("Ruta Nacional 9")]
    public void Validate_WithValidStreetFormats_ShouldNotHaveError(string validStreet)
    {
        // Arrange: Crear request con diferentes formatos válidos de calle
        var request = new CreateAddressRequest(validStreet, "1500", "Buenos Aires", "Vicente López");

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveValidationErrorFor(x => x.Street);
    }

    #endregion

    #region Number Validation Tests

    // Test - Número vacío debe fallar validación
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithEmptyNumber_ShouldHaveError(string invalidNumber)
    {
        // Arrange: Crear request con número inválido
        var request = new CreateAddressRequest("Av. Libertador", invalidNumber, "Buenos Aires", "Vicente López");

        // Act & Assert: Validar y verificar error en número
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Number)
            .WithErrorMessage("El número es requerido");
    }

    // Test - Número muy largo debe fallar validación
    [Fact]
    public void Validate_WithTooLongNumber_ShouldHaveError()
    {
        // Arrange: Crear número de 6 caracteres (más del límite de 5)
        var longNumber = "123456";
        var request = new CreateAddressRequest("Av. Libertador", longNumber, "Buenos Aires", "Vicente López");

        // Act & Assert: Validar y verificar error de longitud
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Number)
            .WithErrorMessage("El número no puede tener más de 5 caracteres");
    }

    // Test - Número con formato inválido debe fallar validación
    [Theory]
    [InlineData("123@")]
    [InlineData("12#3")]
    [InlineData("número")]
    [InlineData("123!")]
    [InlineData("12%34")]
    public void Validate_WithInvalidNumberFormat_ShouldHaveError(string invalidNumber)
    {
        // Arrange: Crear request con número con formato inválido
        var request = new CreateAddressRequest("Av. Libertador", invalidNumber, "Buenos Aires", "Vicente López");

        // Act & Assert: Validar y verificar error de formato
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Number)
            .WithErrorMessage("El número tiene un formato inválido (ej: 123, 123A, 123-125)");
    }

    // Test - Números válidos con diferentes formatos deben pasar validación
    [Theory]
    [InlineData("1500")]
    [InlineData("150A")]
    [InlineData("150-2")]
    [InlineData("15/2")]
    [InlineData("15 A")]
    [InlineData("123")]
    [InlineData("12345")] // Exactamente 5 caracteres
    public void Validate_WithValidNumberFormats_ShouldNotHaveError(string validNumber)
    {
        // Arrange: Crear request con diferentes formatos válidos de número
        var request = new CreateAddressRequest("Av. Libertador", validNumber, "Buenos Aires", "Vicente López");

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
    }

    #endregion

    #region Province Validation Tests

    // Test - Provincia vacía debe fallar validación
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithEmptyProvince_ShouldHaveError(string invalidProvince)
    {
        // Arrange: Crear request con provincia inválida
        var request = new CreateAddressRequest("Av. Libertador", "1500", invalidProvince, "Vicente López");

        // Act & Assert: Validar y verificar error en provincia
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Province)
            .WithErrorMessage("La provincia es requerida");
    }

    // Test - Provincia muy larga debe fallar validación
    [Fact]
    public void Validate_WithTooLongProvince_ShouldHaveError()
    {
        // Arrange: Crear provincia de 101 caracteres (más del límite de 100)
        var longProvince = new string('P', 101);
        var request = new CreateAddressRequest("Av. Libertador", "1500", longProvince, "Vicente López");

        // Act & Assert: Validar y verificar error de longitud
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Province)
            .WithErrorMessage("La provincia no puede tener más de 100 caracteres");
    }

    // Test - Provincias válidas deben pasar validación
    [Theory]
    [InlineData("Buenos Aires")]
    [InlineData("Córdoba")]
    [InlineData("Santa Fe")]
    [InlineData("Ciudad Autónoma de Buenos Aires")]
    [InlineData("Tierra del Fuego")]
    public void Validate_WithValidProvinces_ShouldNotHaveError(string validProvince)
    {
        // Arrange: Crear request con diferentes provincias válidas
        var request = new CreateAddressRequest("Av. Libertador", "1500", validProvince, "Vicente López");

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveValidationErrorFor(x => x.Province);
    }

    #endregion

    #region City Validation Tests

    // Test - Ciudad vacía debe fallar validación
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithEmptyCity_ShouldHaveError(string invalidCity)
    {
        // Arrange: Crear request con ciudad inválida
        var request = new CreateAddressRequest("Av. Libertador", "1500", "Buenos Aires", invalidCity);

        // Act & Assert: Validar y verificar error en ciudad
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.City)
            .WithErrorMessage("La ciudad es requerida");
    }

    // Test - Ciudad muy larga debe fallar validación
    [Fact]
    public void Validate_WithTooLongCity_ShouldHaveError()
    {
        // Arrange: Crear ciudad de 101 caracteres (más del límite de 100)
        var longCity = new string('C', 101);
        var request = new CreateAddressRequest("Av. Libertador", "1500", "Buenos Aires", longCity);

        // Act & Assert: Validar y verificar error de longitud
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.City)
            .WithErrorMessage("La ciudad no puede tener más de 100 caracteres");
    }

    // Test - Ciudades válidas deben pasar validación
    [Theory]
    [InlineData("Vicente López")]
    [InlineData("Capital Federal")]
    [InlineData("La Plata")]
    [InlineData("San Carlos de Bariloche")]
    [InlineData("Río Gallegos")]
    public void Validate_WithValidCities_ShouldNotHaveError(string validCity)
    {
        // Arrange: Crear request con diferentes ciudades válidas
        var request = new CreateAddressRequest("Av. Libertador", "1500", "Buenos Aires", validCity);

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveValidationErrorFor(x => x.City);
    }

    #endregion

    #region Complete Address Validation Tests

    // Test - Dirección completa con todos los campos en el límite debe pasar validación
    [Fact]
    public void Validate_WithAllFieldsAtMaxLength_ShouldNotHaveErrors()
    {
        // Arrange: Crear request con todos los campos en su longitud máxima
        var maxStreet = new string('S', 150);
        var maxNumber = "12345"; // 5 caracteres exacto
        var maxProvince = new string('P', 100);
        var maxCity = new string('C', 100);

        var request = new CreateAddressRequest(maxStreet, maxNumber, maxProvince, maxCity);

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Test - Múltiples errores deben ser reportados simultáneamente
    [Fact]
    public void Validate_WithMultipleErrors_ShouldReportAllErrors()
    {
        // Arrange: Crear request con múltiples campos inválidos
        var request = new CreateAddressRequest("", "123456", "", ""); // Calle vacía, número muy largo, provincia y ciudad vacías

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: Debe tener errores en múltiples campos
        result.ShouldHaveValidationErrorFor(x => x.Street);
        result.ShouldHaveValidationErrorFor(x => x.Number);
        result.ShouldHaveValidationErrorFor(x => x.Province);
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    #endregion
}