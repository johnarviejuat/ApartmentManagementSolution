using FluentValidation;
namespace Catalog.Application.Apartments.Commands.Create;

public sealed class CreateApartmentValidator : AbstractValidator<CreateApartmentCommand>
{
    public CreateApartmentValidator()
    {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150).WithMessage("Name is required and cannot exceed 150 characters");
            RuleFor(x => x.UnitNumber).GreaterThan(0).WithMessage("Unit number must be greater than zero");
            RuleFor(x => x.Line1).NotEmpty().MaximumLength(200).WithMessage("Address line 1 is required and cannot exceed 200 characters");
            RuleFor(x => x.City).NotEmpty().MaximumLength(100).WithMessage("City is required and cannot exceed 100 characters");
            RuleFor(x => x.State).NotEmpty().MaximumLength(50).WithMessage("State is required and cannot exceed 50 characters");
            RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(20).WithMessage("Postal code is required and cannot exceed 20 characters");
            RuleFor(x => x.Bedrooms).GreaterThanOrEqualTo(0).WithMessage("Bedrooms must be zero or more");
            RuleFor(x => x.Bathrooms).GreaterThanOrEqualTo(0).WithMessage("Bathrooms must be zero or more");
            RuleFor(x => x.Capacity).GreaterThanOrEqualTo(0).WithMessage("Capacity must be one or more");
            RuleFor(x => x.SquareFeet)
                .GreaterThan(0)
                .When(x => x.SquareFeet.HasValue)
                .WithMessage("Square feet must be greater than zero when specified");

            RuleFor(x => x.MonthlyRent)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Monthly rent must be zero or more");

            RuleFor(x => x.AdvanceRent)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Advance rent must be zero or more");

            RuleFor(x => x.SecurityDeposit)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Security must be zero or more");

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage("Description cannot exceed 2000 characters");
    }
}
