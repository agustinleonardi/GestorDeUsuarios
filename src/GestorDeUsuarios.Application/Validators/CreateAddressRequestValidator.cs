using FluentValidation;
using GestorDeUsuarios.Application.Models.Requests;

namespace GestorDeUsuarios.Application.Validators;

public class CreateAddressRequestValidator : AbstractValidator<CreateAddressRequest>
{
    public CreateAddressRequestValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("La calle es requerida")
            .MaximumLength(150).WithMessage("La calle no puede tener más de 150 caracteres");

        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("El número es requerido")
            .MaximumLength(5).WithMessage("El número no puede tener más de 5 caracteres")
            .Matches(@"^[0-9a-zA-Z\s\-\/]+$").WithMessage("El número tiene un formato inválido (ej: 123, 123A, 123-125)");

        RuleFor(x => x.Province)
            .NotEmpty().WithMessage("La provincia es requerida")
            .MaximumLength(100).WithMessage("La provincia no puede tener más de 100 caracteres");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("La ciudad es requerida")
            .MaximumLength(100).WithMessage("La ciudad no puede tener más de 100 caracteres");
    }
}