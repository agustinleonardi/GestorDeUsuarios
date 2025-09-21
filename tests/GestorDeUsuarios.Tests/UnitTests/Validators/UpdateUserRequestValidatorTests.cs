using FluentAssertions;
using FluentValidation.TestHelper;
using GestorDeUsuarios.Application.Models.Requests;
using GestorDeUsuarios.Application.Validators;
using Xunit;

namespace GestorDeUsuarios.Tests.UnitTests.Validators;

public class UpdateUserRequestValidatorTests
{
    private readonly UpdateUserRequestValidator _validator;

    public UpdateUserRequestValidatorTests()
    {
        _validator = new UpdateUserRequestValidator();
    }

    // Test - Request de actualización con datos válidos debe pasar validación
    [Fact]
    public void Validate_WithValidUpdateRequest_ShouldNotHaveErrors()
    {
        // Arrange: Crear request de actualización con datos completamente válidos
        var request = new UpdateUserRequest(
            "Carlos López",
            "carlos@example.com",
            new UpdateAddressRequest("Av. Santa Fe", "5678", "Buenos Aires", "Palermo")
        );

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Test - Request de actualización sin dirección debe pasar validación
    [Fact]
    public void Validate_WithoutAddress_ShouldNotHaveErrors()
    {
        // Arrange: Crear request de actualización sin dirección
        var request = new UpdateUserRequest("Ana Silva", "ana@example.com", null);

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores (dirección es opcional)
        result.ShouldNotHaveAnyValidationErrors();
    }

    #region Name Validation Tests

    // Test - Nombre vacío debe fallar validación
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithEmptyName_ShouldHaveError(string invalidName)
    {
        // Arrange: Crear request con nombre inválido
        var request = new UpdateUserRequest(invalidName, "test@example.com", null);

        // Act & Assert: Validar y verificar error en nombre
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("El nombre es requerido");
    }

    // Test - Nombre muy largo debe fallar validación
    [Fact]
    public void Validate_WithTooLongName_ShouldHaveError()
    {
        // Arrange: Crear nombre de 101 caracteres (más del límite de 100)
        var longName = new string('B', 101);
        var request = new UpdateUserRequest(longName, "test@example.com", null);

        // Act & Assert: Validar y verificar error de longitud
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("El nombre no puede tener más de 100 caracteres");
    }

    // Test - Nombre con caracteres inválidos debe fallar validación
    [Theory]
    [InlineData("Carlos123")]
    [InlineData("Laura@Martínez")]
    [InlineData("Diego#Rodríguez")]
    public void Validate_WithInvalidNameCharacters_ShouldHaveError(string invalidName)
    {
        // Arrange: Crear request con nombre con caracteres no permitidos
        var request = new UpdateUserRequest(invalidName, "test@example.com", null);

        // Act & Assert: Validar y verificar error de formato
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("El nombre solo puede contener letras y espacios");
    }

    #endregion

    #region Email Validation Tests

    // Test - Email vacío debe fallar validación
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithEmptyEmail_ShouldHaveError(string invalidEmail)
    {
        // Arrange: Crear request con email inválido
        var request = new UpdateUserRequest("Carlos López", invalidEmail, null);

        // Act & Assert: Validar y verificar error en email
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("El email es requerido");
    }

    // Test - Email con formato inválido debe fallar validación
    [Theory]
    [InlineData("invalid-email-update")]
    [InlineData("@update.com")]
    [InlineData("user@")]
    [InlineData("user.update.com")]
    public void Validate_WithInvalidEmailFormat_ShouldHaveError(string invalidEmail)
    {
        // Arrange: Crear request con formato de email inválido
        var request = new UpdateUserRequest("Carlos López", invalidEmail, null);

        // Act & Assert: Validar y verificar error de formato de email
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("El formato del email no es válido");
    }

    // Test - Email muy largo debe fallar validación
    [Fact]
    public void Validate_WithTooLongEmail_ShouldHaveError()
    {
        // Arrange: Crear email muy largo (más de 200 caracteres)
        var longEmail = new string('x', 190) + "@update.com"; // 201 chars total
        var request = new UpdateUserRequest("Carlos López", longEmail, null);

        // Act & Assert: Validar y verificar error de longitud
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("El email no puede tener más de 200 caracteres");
    }

    #endregion

    #region Address Validation Tests (UpdateAddressRequest)

    // Test - Dirección con calle vacía debe fallar validación
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithEmptyAddressCalle_ShouldHaveError(string invalidCalle)
    {
        // Arrange: Crear request con dirección con calle inválida
        var address = new UpdateAddressRequest(invalidCalle, "5678", "Buenos Aires", "Palermo");
        var request = new UpdateUserRequest("Carlos López", "carlos@example.com", address);

        // Act & Assert: Validar y verificar error en calle
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("Address.Street")
            .WithErrorMessage("La calle es requerida cuando se proporciona domicilio");
    }

    // Test - Dirección con número vacío debe fallar validación
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithEmptyAddressNumero_ShouldHaveError(string invalidNumero)
    {
        // Arrange: Crear request con dirección con número inválido
        var address = new UpdateAddressRequest("Av. Santa Fe", invalidNumero, "Buenos Aires", "Palermo");
        var request = new UpdateUserRequest("Carlos López", "carlos@example.com", address);

        // Act & Assert: Validar y verificar error en número
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("Address.Number")
            .WithErrorMessage("El número es requerido cuando se proporciona domicilio");
    }

    // Test - Dirección con número con formato inválido debe fallar validación
    [Theory]
    [InlineData("567@")]
    [InlineData("56#78")]
    [InlineData("número")]
    public void Validate_WithInvalidAddressNumeroFormat_ShouldHaveError(string invalidNumero)
    {
        // Arrange: Crear request con número de dirección con formato inválido
        var address = new UpdateAddressRequest("Av. Santa Fe", invalidNumero, "Buenos Aires", "Palermo");
        var request = new UpdateUserRequest("Carlos López", "carlos@example.com", address);

        // Act & Assert: Validar y verificar error de formato en número
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("Address.Number")
            .WithErrorMessage("El número debe ser un formato válido (ej: 123, 123A, 123-125, 123/45)");
    }

    // Test - Números de dirección válidos deben pasar validación
    [Theory]
    [InlineData("5678")]
    [InlineData("567")]
    [InlineData("5679")]
    [InlineData("5678")]
    [InlineData("567")]
    public void Validate_WithValidAddressNumero_ShouldNotHaveError(string validNumero)
    {
        // Arrange: Crear request con número de dirección válido
        var address = new UpdateAddressRequest("Av. Santa Fe", validNumero, "Buenos Aires", "Palermo");
        var request = new UpdateUserRequest("Carlos López", "carlos@example.com", address);

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores en el número
        result.ShouldNotHaveValidationErrorFor("Address.Number");
    }

    // Test - Dirección con provincia vacía debe fallar validación
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithEmptyAddressProvincia_ShouldHaveError(string invalidProvincia)
    {
        // Arrange: Crear request con dirección con provincia inválida
        var address = new UpdateAddressRequest("Av. Santa Fe", "5678", invalidProvincia, "Palermo");
        var request = new UpdateUserRequest("Carlos López", "carlos@example.com", address);

        // Act & Assert: Validar y verificar error en provincia
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("Address.Province")
            .WithErrorMessage("La provincia es requerida cuando se proporciona domicilio");
    }

    // Test - Dirección con ciudad vacía debe fallar validación
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithEmptyAddressCiudad_ShouldHaveError(string invalidCiudad)
    {
        // Arrange: Crear request con dirección con ciudad inválida
        var address = new UpdateAddressRequest("Av. Santa Fe", "5678", "Buenos Aires", invalidCiudad);
        var request = new UpdateUserRequest("Carlos López", "carlos@example.com", address);

        // Act & Assert: Validar y verificar error en ciudad
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("Address.City")
            .WithErrorMessage("La ciudad es requerida cuando se proporciona domicilio");
    }

    #endregion
}