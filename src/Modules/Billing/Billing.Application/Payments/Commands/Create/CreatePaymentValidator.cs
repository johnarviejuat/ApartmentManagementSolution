using Billing.Application.Payments.Commands.Create;
using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using FluentValidation;
using Leasing.Domain.Abstraction;
using People.Domain.Abstraction;
using People.Domain.Entities;

namespace Billing.Application.Payments.Commands.Create;
public sealed class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand>
{
    private static readonly string[] AllowedMethods =
    {
        "cash","card","banktransfer","check","mobile"
    };

    public CreatePaymentValidator(
        ITenantRepository tenantRepo,
        IApartmentRepository apartmentRepo,
        ILeaseRepository leaseRepo)
    {
        // apply Stop cascade to all rules by default
        RuleLevelCascadeMode = CascadeMode.Stop;

        // Method
        RuleFor(c => c.Method)
            .NotEmpty().WithMessage("Method is required.")
            .MaximumLength(50)
            .Must(m => AllowedMethods.Contains(m.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage(c => $"Method '{c.Method}' is not supported. Allowed: {string.Join(", ", AllowedMethods)}.");

        // Amount
        RuleFor(c => c.Amount)
            .GreaterThan(0m).WithMessage("Amount must be greater than zero.");

        // TenantId (null/default guard + existence)
        RuleFor(c => c.TenantId)
            .NotNull().WithMessage("TenantId is required.")
            .Must(id => id.Value != Guid.Empty).WithMessage("TenantId cannot be empty.")
            .MustAsync(async (id, ct) =>
            {
                var tenant = await tenantRepo.GetByIdAsync(new TenantId(id.Value), ct);
                return tenant is not null;
            })
            .WithMessage(c => $"Tenant '{c.TenantId.Value}' was not found.");

        // ApartmentId (null/default guard + existence)
        RuleFor(c => c.ApartmentId)
            .NotNull().WithMessage("ApartmentId is required.")
            .Must(id => id.Value != Guid.Empty).WithMessage("ApartmentId cannot be empty.")
            .MustAsync(async (id, ct) =>
            {
                var apt = await apartmentRepo.GetByIdAsync(new ApartmentId(id.Value), ct);
                return apt is not null;
            })
            .WithMessage(c => $"Apartment '{c.ApartmentId.Value}' was not found.");

        // First-payment rule: only when NO active lease exists for tenant+apartment
        WhenAsync(async (c, ct) =>
        {
            // Only run if IDs are present (guards above will short-circuit on failure)
            return c.TenantId != Guid.Empty
                   && c.ApartmentId != Guid.Empty
                   && c.TenantId.Value != Guid.Empty
                   && c.ApartmentId.Value != Guid.Empty
                   && await leaseRepo.GetActiveAsync(c.TenantId.Value, c.ApartmentId.Value, ct) is null;
        },
        () =>
        {
            RuleFor(c => c)
                .CustomAsync(async (c, ctx, ct) =>
                {
                    var apt = await apartmentRepo.GetByIdAsync(new ApartmentId(c.ApartmentId!.Value), ct);
                    if (apt is null)
                    {
                        // defensive: existence rule above should have caught this
                        ctx.AddFailure("ApartmentId", $"Apartment '{c.ApartmentId.Value}' was not found.");
                        return;
                    }

                    static decimal San(decimal x) => x < 0 ? 0 : x;
                    static decimal L(decimal x) => decimal.Round(x, 2, MidpointRounding.AwayFromZero);

                    var advance = L(San(apt.AdvanceRent));
                    var deposit = L(San(apt.SecurityDeposit));
                    var required = L(advance + deposit);

                    if (required <= 0m) return; // nothing required for move-in

                    if (L(c.Amount) != required)
                    {
                        ctx.AddFailure("Amount",
                            $"First payment must equal Advance + Deposit: {required:N2}. " +
                            $"You sent {L(c.Amount):N2}. Breakdown — Advance: {advance:N2}, Deposit: {deposit:N2}.");
                    }
                });
        });

        // Notes
        RuleFor(c => c.Notes).MaximumLength(1000);
    }
}
