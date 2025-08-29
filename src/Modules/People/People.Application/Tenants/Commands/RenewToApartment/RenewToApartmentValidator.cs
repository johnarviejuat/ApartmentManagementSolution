using FluentValidation;

namespace People.Application.Tenants.Commands.RenewToApartment;

public sealed class RenewToApartmentValidator : AbstractValidator<RenewToaApartmentCommand>
{
    public RenewToApartmentValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.ApartmentId).NotEmpty();
        RuleFor(x => x.NewStartDate).NotEmpty();
        RuleFor(x => x.NewMonthlyRent).GreaterThan(0m);
        When(x => x.NewDepositRequired.HasValue, () =>
        {
            RuleFor(x => x.NewDepositRequired!.Value).GreaterThanOrEqualTo(0m);
        });
    }
}
