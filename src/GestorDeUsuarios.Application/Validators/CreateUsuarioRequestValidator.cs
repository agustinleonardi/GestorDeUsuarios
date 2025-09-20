using FluentValidation;
using GestorDeUsuarios.Application.Models.Requests;

namespace GestorDeUsuarios.Application.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede tener más de 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$").WithMessage("El nombre solo puede contener letras y espacios");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido")
            .MaximumLength(200).WithMessage("El email no puede tener más de 200 caracteres");

        // Validación condicional del domicilio
        When(x => x.Address != null, () =>
        {
            RuleFor(x => x.Address!.Street)
                .NotEmpty().WithMessage("La calle es requerida cuando se proporciona domicilio")
                .MaximumLength(150).WithMessage("La calle no puede tener más de 150 caracteres");

            RuleFor(x => x.Address!.Number)
                .NotEmpty().WithMessage("El número es requerido cuando se proporciona domicilio")
                .MaximumLength(20).WithMessage("El número no puede tener más de 20 caracteres")
                .Matches(@"^[0-9a-zA-Z\s\-\/]+$").WithMessage("El número tiene un formato inválido (ej: 123, 123A, 123-125)");

            RuleFor(x => x.Address!.Province)
                .NotEmpty().WithMessage("La provincia es requerida cuando se proporciona domicilio")
                .MaximumLength(100).WithMessage("La provincia no puede tener más de 100 caracteres");

            RuleFor(x => x.Address!.City)
                .NotEmpty().WithMessage("La ciudad es requerida cuando se proporciona domicilio")
                .MaximumLength(100).WithMessage("La ciudad no puede tener más de 100 caracteres");
        });
    }
}