using Billing.Application.Payments.Commands.Create;
using Billing.Domain.Abstraction;
using Billing.Domain.Entities;
using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using Leasing.Domain.Abstraction;
using Leasing.Domain.Entities;
using FluentValidation;
using MediatR;
using People.Domain.Entities;

namespace Billing.Application.Payments;

public sealed class CreatePaymentHandler(
    IPaymentRepository paymentRepo,
    ILeaseRepository leaseRepo,
    IApartmentRepository apartmentRepo,
    IValidator<CreatePaymentCommand> validator)
    : IRequestHandler<CreatePaymentCommand, Guid>
{
    private readonly IPaymentRepository _paymentRepo = paymentRepo;
    private readonly ILeaseRepository _leaseRepo = leaseRepo;
    private readonly IApartmentRepository _apartmentRepo = apartmentRepo;
    private readonly IValidator<CreatePaymentCommand> _validator = validator;

    private sealed record LeaseApplyResult(
        DateOnly CoverageStart,
        int MonthsCovered,
        DateOnly NewNextDueDate,
        decimal RentPortion,
        decimal DepositPortion,
        decimal NewCredit
    );

    public async Task<Guid> Handle(CreatePaymentCommand c, CancellationToken ct)
    {
        var vr = await _validator.ValidateAsync(c, ct);
        if (!vr.IsValid) throw new ValidationException(vr.Errors);

        var reference = await GenerateUniqueReferenceAsync(ct);

        var payment = new Payment(
            tenantId: c.TenantId.Value,
            amount: c.Amount,
            method: c.Method,
            apartmentId: c.ApartmentId.Value,
            referenceNumber: reference,
            notes: c.Notes
        );

        const int maxAttempts = 3;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                // 1) persist payment
                await _paymentRepo.AddAsync(payment, ct);
                await _paymentRepo.SaveChangesAsync(ct);

                // 2) apply to lease
                var apply = await UpsertLeaseAndApplyAsync(new TenantId(c.TenantId.Value), new ApartmentId(c.ApartmentId.Value), payment.Amount, ct, createIfMissing: true);

                // 3) optionally tag deposit portion
                if (apply.DepositPortion > 0m)
                {
                    payment.MarkDepositPortion(apply.DepositPortion);
                    await _paymentRepo.SaveChangesAsync(ct);
                }

                // if needed:
                var paidNextDue = apply.MonthsCovered >= 1;
                _ = paidNextDue;

                return payment.Id;
            }
            catch (Exception ex) when (IsUniqueRefViolation(ex) && attempt < maxAttempts)
            {
                reference = PaymentReference.New();
                payment = new Payment(
                    tenantId: c.TenantId.Value,
                    amount: c.Amount,
                    method: c.Method,
                    apartmentId: c.ApartmentId.Value,
                    referenceNumber: reference,
                    notes: c.Notes
                );
            }
        }

        throw new InvalidOperationException("Failed to persist payment after retries.");
    }

    private async Task<LeaseApplyResult> UpsertLeaseAndApplyAsync(
        TenantId tenantId,
        ApartmentId apartmentId,
        decimal amount,
        CancellationToken ct,
        bool createIfMissing = false)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(amount, 0m);

        // tracked entity
        var lease = await _leaseRepo.GetActiveAsync(tenantId.Value, apartmentId.Value, ct);

        if (lease is null)
        {
            if (!createIfMissing)
                throw new InvalidOperationException(
                    $"No active lease for Tenant '{tenantId}' and Apartment '{apartmentId}'.");

            // check any history (active or inactive)
            var hasHistory = await _leaseRepo.ExistsForTenantApartmentAsync(tenantId.Value, apartmentId.Value, ct);
            if (hasHistory)
                throw new InvalidOperationException(
                    "No active lease exists but history was found. Use the renew endpoint before taking payment.");

            var apt = await _apartmentRepo.GetByIdAsync(apartmentId, ct)
                      ?? throw new InvalidOperationException($"Apartment '{apartmentId}' not found.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var available = apt.AvailableFrom ?? today;
            var startDate = available > today ? available : today;

            var firstDue = ComputeFirstDueDate(startDate);
            var depositRequired = apt.SecurityDeposit;

            lease = new Lease(tenantId.Value, apartmentId.Value, apt.MonthlyRent, firstDue, depositRequired, startDate);
            await _leaseRepo.AddAsync(lease, ct);
        }

        var coverageStart = lease.NextDueDate;

        // 1) deposit first
        var depositNeeded = Math.Max(0m, lease.DepositRequired - lease.DepositHeld);
        var depositPortion = Math.Min(depositNeeded, amount);
        if (depositPortion > 0m)
            lease.FundDeposit(depositPortion);

        // 2) then rent — APPLY IMMEDIATELY so due date advances on early pay
        var rentPortion = amount - depositPortion;

        int monthsCovered;
        decimal remainder;
        if (rentPortion > 0m)
            (monthsCovered, remainder) = lease.ApplyPayment(rentPortion);
        else
            (monthsCovered, remainder) = (0, lease.Credit);

        await _leaseRepo.SaveChangesAsync(ct);

        return new LeaseApplyResult(
            CoverageStart: coverageStart,
            MonthsCovered: monthsCovered,
            NewNextDueDate: lease.NextDueDate,
            RentPortion: rentPortion,
            DepositPortion: depositPortion,
            NewCredit: remainder
        );
    }

    private static DateOnly ComputeFirstDueDate(DateOnly availableFrom)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var anchor = availableFrom;

        var sameMonthDay = Math.Min(anchor.Day, DateTime.DaysInMonth(today.Year, today.Month));
        var due = new DateOnly(today.Year, today.Month, sameMonthDay);

        if (due < today)
        {
            var next = today.AddMonths(1);
            var nextDay = Math.Min(anchor.Day, DateTime.DaysInMonth(next.Year, next.Month));
            due = new DateOnly(next.Year, next.Month, nextDay);
        }

        return due;
    }
    private async Task<string> GenerateUniqueReferenceAsync(CancellationToken ct)
    {
        for (var i = 0; i < 5; i++)
        {
            var candidate = PaymentReference.New();
            if (!await _paymentRepo.ExistsByReferenceAsync(candidate, ct))
                return candidate;
        }
        return PaymentReference.New();
    }
    private static bool IsUniqueRefViolation(Exception ex)
    {
        var e = ex.InnerException ?? ex;
        e = e.InnerException ?? e;

        switch (e.GetType().Name)
        {
            case "SqlException":
                var number = (int?)e.GetType().GetProperty("Number")?.GetValue(e);
                return number is 2601 or 2627;     // duplicate key
            case "PostgresException":
                var state = (string?)e.GetType().GetProperty("SqlState")?.GetValue(e);
                return state == "23505";
            case "SqliteException":
                var code = (int?)e.GetType().GetProperty("SqliteErrorCode")?.GetValue(e);
                return code == 19;
            default:
                return false;
        }
    }
}
