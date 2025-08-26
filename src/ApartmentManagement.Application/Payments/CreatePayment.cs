using ApartmentManagement.Application.Payments.Commands.Create;
using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Leases;
using ApartmentManagement.Domain.Leasing.Payments;
using ApartmentManagement.Domain.Leasing.Tenants;
using FluentValidation;
using MediatR;

namespace ApartmentManagement.Application.Payments;
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
            tenantId: c.TenantId,
            amount: c.Amount,
            method: c.Method,
            apartmentId: c.ApartmentId,
            referenceNumber: reference,
            notes: c.Notes
        );

        const int maxAttempts = 3;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                // 1) persist the payment
                await _paymentRepo.AddAsync(payment, ct);
                await _paymentRepo.SaveChangesAsync(ct);

                // 2) apply to lease and capture what happened
                var apply = await UpsertLeaseAndApplyAsync(c.TenantId, c.ApartmentId, payment.Amount, ct);

                // 3) (optional) if you already store DepositPortion on Payment, tag it:
                if (apply.DepositPortion > 0m)
                {
                    payment.MarkDepositPortion(apply.DepositPortion);
                    await _paymentRepo.SaveChangesAsync(ct);
                }

                // 4) you now know if the "next due" was paid:
                var paidNextDue = apply.MonthsCovered >= 1;

                // If you want to RETURN these details from the API,
                // change your controller to include them (see next section).
                return payment.Id;
            }
            catch (Exception ex) when (IsUniqueRefViolation(ex) && attempt < maxAttempts)
            {
                reference = PaymentReference.New();
                payment = new Payment(
                    tenantId: c.TenantId,
                    amount: c.Amount,
                    method: c.Method,
                    apartmentId: c.ApartmentId,
                    referenceNumber: reference,
                    notes: c.Notes
                );
            }
        }

        throw new InvalidOperationException("Failed to persist payment after retries.");
    }

    private async Task<LeaseApplyResult> UpsertLeaseAndApplyAsync(
     TenantId tenantId, ApartmentId apartmentId, decimal amount, CancellationToken ct)
    {
        var lease = await _leaseRepo.GetActiveAsync(tenantId, apartmentId, ct);
        if (lease is null)
        {
            var apt = await _apartmentRepo.GetByIdAsync(apartmentId, ct)
                      ?? throw new InvalidOperationException($"Apartment '{apartmentId.Value}' not found when creating lease.");

            var firstDue = ComputeFirstDueDate(apt.AvailableFrom);

            // Choose how you derive required deposit (absolute vs "months"):
            var depositRequired = apt.SecurityDeposit;             // absolute amount
                                                                   // var depositRequired = apt.MonthlyRent * apt.SecurityDeposit; // if "months"

            lease = new Lease(tenantId, apartmentId, apt.MonthlyRent, firstDue, depositRequired);
            await _leaseRepo.AddAsync(lease, ct);
        }

        var startDue = lease.NextDueDate;

        // Hold deposit first (if not fully funded), rest goes to rent
        var needDeposit = Math.Max(0m, lease.DepositRequired - lease.DepositHeld);
        var depositPart = Math.Min(needDeposit, amount);
        var rentPart = amount - depositPart;

        if (depositPart > 0m)
            lease.DepositHeld = decimal.Round(lease.DepositHeld + depositPart, 2, MidpointRounding.AwayFromZero);

        var (monthsCovered, remainder) = lease.ApplyPayment(rentPart);

        await _leaseRepo.SaveChangesAsync(ct);

        return new LeaseApplyResult(
            CoverageStart: startDue,
            MonthsCovered: monthsCovered,
            NewNextDueDate: lease.NextDueDate,
            RentPortion: rentPart,
            DepositPortion: depositPart,
            NewCredit: remainder
        );
    }


    private static DateOnly ComputeFirstDueDate(DateOnly? availableFrom)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var anchor = availableFrom ?? today;

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
                {
                    var number = (int?)e.GetType().GetProperty("Number")?.GetValue(e);
                    return number is 2601 or 2627;
                }
            case "PostgresException":
                {
                    var state = (string?)e.GetType().GetProperty("SqlState")?.GetValue(e);
                    return state == "23505";
                }
            case "SqliteException":
                {
                    var code = (int?)e.GetType().GetProperty("SqliteErrorCode")?.GetValue(e);
                    return code == 19;
                }
            default:
                return false;
        }
    }
}
 