using FluentValidation;
using GestorDeUsuarios.Application.Models.Requests;

namespace GestorDeUsuarios.Application.Validators;

public class SearchUsersRequestValidator : AbstractValidator<SearchUsersRequest>
{
    public SearchUsersRequestValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.Name), () =>
                {
                    RuleFor(x => x.Name).MaximumLength(100);
                });

        When(x => !string.IsNullOrWhiteSpace(x.Province), () =>
        {
            RuleFor(x => x.Province).MaximumLength(100);
        });

        When(x => !string.IsNullOrWhiteSpace(x.City), () =>
        {
            RuleFor(x => x.City).MaximumLength(100);
        });
    }
}