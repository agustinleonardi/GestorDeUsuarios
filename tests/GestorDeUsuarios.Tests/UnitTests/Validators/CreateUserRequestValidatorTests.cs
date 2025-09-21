using FluentAssertions;
using FluentValidation.TestHelper;
using GestorDeUsuarios.Application.Models.Requests;
using GestorDeUsuarios.Application.Validators;
using Xunit;

namespace GestorDeUsuarios.Tests.UnitTests.Validators;

public class CreateUserRequestValidatorTests
{
    private readonly CreateUserRequestValidator _validator;

    public CreateUserRequestValidatorTests()
    {
        _validator = new CreateUserRequestValidator();
    }

    // Test - Request con datos válidos debe pasar validación
    [Fact]
    public void Validate_WithValidRequest_ShouldNotHaveErrors()
    {
        // Arrange: Crear request con datos completamente válidos
        var request = new CreateUserRequest(
            "Juan Pérez",
            "juan@example.com",
            new CreateAddressRequest("Av. Corrientes", "1234", "Buenos Aires", "CABA")
        );

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Test - Request sin dirección debe pasar validación (dirección es opcional)
    [Fact]
    public void Validate_WithoutAddress_ShouldNotHaveErrors()
    {
        // Arrange: Crear request sin dirección
        var request = new CreateUserRequest("María García", "maria@example.com", null);

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
        var request = new CreateUserRequest(invalidName, "test@example.com", null);

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
        var longName = new string('A', 101);
        var request = new CreateUserRequest(longName, "test@example.com", null);

        // Act & Assert: Validar y verificar error de longitud
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("El nombre no puede tener más de 100 caracteres");
    }

    // Test - Nombre con caracteres inválidos debe fallar validación
    [Theory]
    [InlineData("Juan123")]
    [InlineData("María@García")]
    [InlineData("Pedro#López")]
    [InlineData("Ana$Silva")]
    public void Validate_WithInvalidNameCharacters_ShouldHaveError(string invalidName)
    {
        // Arrange: Crear request con nombre con caracteres no permitidos
        var request = new CreateUserRequest(invalidName, "test@example.com", null);

        // Act & Assert: Validar y verificar error de formato
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("El nombre solo puede contener letras y espacios");
    }

    // Test - Nombres válidos con acentos y espacios deben pasar validación
    [Theory]
    [InlineData("José María")]
    [InlineData("María José García")]
    [InlineData("Ñoño")]
    [InlineData("François")]
    [InlineData("João")]
    public void Validate_WithValidNameCharacters_ShouldNotHaveError(string validName)
    {
        // Arrange: Crear request con nombre válido con caracteres especiales
        var request = new CreateUserRequest(validName, "test@example.com", null);

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores en el nombre
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
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
        var request = new CreateUserRequest("Juan Pérez", invalidEmail, null);

        // Act & Assert: Validar y verificar error en email
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("El email es requerido");
    }

    // Test - Email con formato inválido debe fallar validación
    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user.example.com")]
    public void Validate_WithInvalidEmailFormat_ShouldHaveError(string invalidEmail)
    {
        // Arrange: Crear request con formato de email inválido
        var request = new CreateUserRequest("Juan Pérez", invalidEmail, null);

        // Act & Assert: Validar y verificar error de formato de email
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("El formato del email no es válido");
    }

    // Test - Email muy largo debe fallar validación
    [Fact]
    public void Validate_WithTooLongEmail_ShouldHaveError()
    {
        // Arrange: Crear request con email demasiado largo (más de 200 chars)
        var longEmail = new string('a', 192) + "@test.com"; // 201 chars total
        var request = new CreateUserRequest("Juan Pérez", longEmail, null);

        // Act & Assert: Validar y verificar error de longitud
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("El email no puede tener más de 200 caracteres");
    }

    // Test - Emails válidos deben pasar validación
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("firstname+lastname@company.org")]
    [InlineData("email@123.123.123.123")] // IP válida
    public void Validate_WithValidEmail_ShouldNotHaveError(string validEmail)
    {
        // Arrange: Crear request con email válido
        var request = new CreateUserRequest("Juan Pérez", validEmail, null);

        // Act: Validar el request
        var result = _validator.TestValidate(request);

        // Assert: No debe tener errores en el email
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    #endregion

    #region Address Validation Tests

    // Test - Dirección con calle vacía debe fallar validación
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithEmptyAddressStreet_ShouldHaveError(string invalidStreet)
    {
        // Arrange: Crear request con dirección con calle inválida
        var address = new CreateAddressRequest(invalidStreet, "1234", "Buenos Aires", "CABA");
        var request = new CreateUserRequest("Juan Pérez", "juan@example.com", address);

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
    public void Validate_WithEmptyAddressNumber_ShouldHaveError(string invalidNumber)
    {
        // Arrange: Crear request con dirección con número inválido
        var address = new CreateAddressRequest("Av. Corrientes", invalidNumber, "Buenos Aires", "CABA");
        var request = new CreateUserRequest("Juan Pérez", "juan@example.com", address);

        // Act & Assert: Validar y verificar error en número
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("Address.Number")
            .WithErrorMessage("El número es requerido cuando se proporciona domicilio");
    }

    // Test - Dirección con número con formato inválido debe fallar validación
    [Theory]
    [InlineData("123@")]
    [InlineData("12#34")]
    [InlineData("número")]
    public void Validate_WithInvalidAddressNumberFormat_ShouldHaveError(string invalidNumber)
    {
        // Arrange: Crear request con número de dirección con formato inválido
        var address = new CreateAddressRequest("Av. Corrientes", invalidNumber, "Buenos Aires", "CABA");
        var request = new CreateUserRequest("Juan Pérez", "juan@example.com", address);

        // Act & Assert: Validar y verificar error de formato en número
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("Address.Number")
            .WithErrorMessage("El número debe ser un formato válido (ej: 123, 123A, 123-125, 123/45)");
    }

    // Test - Números de dirección válidos deben pasar validación
    [Theory]
    [InlineData("1234")]
    [InlineData("123")]
    [InlineData("1235")]
    [InlineData("1234")]
    [InlineData("123")]
    public void Validate_WithValidAddressNumber_ShouldNotHaveError(string validNumber)
    {
        // Arrange: Crear request con número de dirección válido
        var address = new CreateAddressRequest("Av. Corrientes", validNumber, "Buenos Aires", "CABA");
        var request = new CreateUserRequest("Juan Pérez", "juan@example.com", address);

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
    public void Validate_WithEmptyAddressProvince_ShouldHaveError(string invalidProvince)
    {
        // Arrange: Crear request con dirección con provincia inválida
        var address = new CreateAddressRequest("Av. Corrientes", "1234", invalidProvince, "CABA");
        var request = new CreateUserRequest("Juan Pérez", "juan@example.com", address);

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
    public void Validate_WithEmptyAddressCity_ShouldHaveError(string invalidCity)
    {
        // Arrange: Crear request con dirección con ciudad inválida
        var address = new CreateAddressRequest("Av. Corrientes", "1234", "Buenos Aires", invalidCity);
        var request = new CreateUserRequest("Juan Pérez", "juan@example.com", address);

        // Act & Assert: Validar y verificar error en ciudad
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("Address.City")
            .WithErrorMessage("La ciudad es requerida cuando se proporciona domicilio");
    }

    #endregion
}