using FluentValidation;

namespace ApartmentManagement.Application.Apartments.Commands.Create;

public sealed class CreateApartmentValidator : AbstractValidator<CreateApartmentCommand>
{
    public CreateApartmentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.UnitNumber)
            .GreaterThan(0).WithMessage("Unit number must be greater than zero");

        RuleFor(x => x.Address.Line1)
            .NotEmpty().WithMessage("Address line is required");

        RuleFor(x => x.Address.PostalCode)
            .NotEmpty().WithMessage("Postal code is required");
    }
}
