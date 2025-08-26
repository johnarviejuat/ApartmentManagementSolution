using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Leases;
using ApartmentManagement.Domain.Leasing.Tenants;
using FluentValidation;

namespace ApartmentManagement.Application.Payments.Commands.Create;

public sealed class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand>
{
    private static readonly string[] AllowedMethods = ["cash", "card", "banktransfer", "check", "mobile"];

    public CreatePaymentValidator(
        ITenantRepository tenantRepo,
        IApartmentRepository apartmentRepo,
        ILeaseRepository leaseRepo)
    {
        // Method
        RuleFor(c => c.Method)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Method is required.")
            .MaximumLength(50)
            .Must(m => m is not null && AllowedMethods.Contains(m.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage(c => $"Method '{c.Method}' is not supported. Allowed: {string.Join(", ", AllowedMethods)}.");

        // Amount
        RuleFor(c => c.Amount)
            .GreaterThan(0m).WithMessage("Amount must be greater than zero.");

        // Tenant must exist
        RuleFor(c => c.TenantId)
            .MustAsync(async (tenantId, ct) =>
            {
                var tenant = await tenantRepo.GetByIdAsync(tenantId, ct);
                return tenant is not null;
            })
            .WithMessage(c => $"Tenant '{c.TenantId.Value}' was not found.");

        // Apartment must exist
        RuleFor(c => c.ApartmentId)
            .MustAsync(async (aptId, ct) =>
            {
                var apt = await apartmentRepo.GetByIdAsync(aptId, ct);
                return apt is not null;
            })
            .WithMessage(c => $"Apartment '{c.ApartmentId.Value}' was not found.");

        // First-payment rule: only when NO lease exists yet for this tenant+apartment
        RuleFor(c => c)
            .CustomAsync(async (c, ctx, ct) =>
            {
                // If a lease already exists, skip the Advance+Deposit rule (subsequent payments)
                var existingLease = await leaseRepo.GetActiveAsync(c.TenantId, c.ApartmentId, ct);
                if (existingLease is not null) return;

                // No lease yet => this is the move-in payment. Enforce Advance + Deposit total.
                var apt = await apartmentRepo.GetByIdAsync(c.ApartmentId, ct);
                if (apt is null)
                {
                    ctx.AddFailure("ApartmentId", $"Apartment '{c.ApartmentId.Value}' was not found.");
                    return;
                }

                // Use absolute amounts from Apartment; guard & round
                static decimal San(decimal x) => x < 0 ? 0 : x;
                static decimal L(decimal x) => decimal.Round(x, 2, MidpointRounding.AwayFromZero);

                var advanceAmount = L(San(apt.AdvanceRent));
                var depositAmount = L(San(apt.SecurityDeposit));
                var requiredTotal = L(advanceAmount + depositAmount);

                if (requiredTotal <= 0m) return; // nothing required for move-in

                if (L(c.Amount) != requiredTotal)
                {
                    ctx.AddFailure("Amount",
                        $"First payment must equal Advance + Deposit: {requiredTotal:N2}. " +
                        $"You sent {L(c.Amount):N2}. Breakdown — Advance: {advanceAmount:N2}, Deposit: {depositAmount:N2}.");
                }
            });

        // Notes
        RuleFor(c => c.Notes).MaximumLength(1000);
    }
}
