using ApartmentManagement.Application.Payments.Commands.Create;
using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Payments;
using ApartmentManagement.Domain.Leasing.Tenants;
using FluentValidation;
using MediatR;

namespace ApartmentManagement.Application.Payments;

public sealed class CreatePaymentHandler(
    IPaymentRepository paymentRepo,
    ITenantRepository tenantRepo,
    IApartmentRepository apartmentRepo,
    IValidator<CreatePaymentCommand> validator
) : IRequestHandler<CreatePaymentCommand, Guid>
{
    private readonly IPaymentRepository _paymentRepo = paymentRepo;
    private readonly ITenantRepository _tenantRepo = tenantRepo;
    private readonly IApartmentRepository _apartmentRepo = apartmentRepo;
    private readonly IValidator<CreatePaymentCommand> _validator = validator;

    public async Task<Guid> Handle(CreatePaymentCommand c, CancellationToken ct)
    {
        // 1) Validate command
        var result = await _validator.ValidateAsync(c, ct);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        // 2) Map incoming ids
        var tenantId = new TenantId(c.TenantId);
        ApartmentId? apartmentId = null;
        if (c.ApartmentId is Guid aptGuid)
            apartmentId = new ApartmentId(aptGuid);

        // 3) Ensure references exist
        _ = await _tenantRepo.GetByIdAsync(tenantId, ct)
            ?? throw new KeyNotFoundException($"Tenant '{tenantId.Value}' was not found.");

        Apartment? apartment = null;
        if (apartmentId is { } aptId)
        {
            apartment = await _apartmentRepo.GetByIdAsync(aptId, ct)
                ?? throw new KeyNotFoundException($"Apartment '{aptId.Value}' was not found.");
        }

        // 4) If an apartment is provided, validate "move-in required amount"
        if (apartment is not null)
        {
            var advanceAmount =
                (apartment.AdvanceRent > 0 ? apartment.AdvanceRent
                                                 : (apartment.MonthlyRent * apartment.AdvanceRent));

            var depositAmount =
                (apartment.SecurityDeposit > 0 ? apartment.SecurityDeposit
                                                     : (apartment.MonthlyRent * apartment.SecurityDeposit));

            var requiredTotal = advanceAmount + depositAmount;
            if (requiredTotal > 0m && c.Amount != requiredTotal)
            {
                var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new("Amount",
                    $"Amount must equal the required move-in total (Advance + Deposit): {requiredTotal:N2}. You sent {c.Amount:N2}. " +
                    $"Breakdown — Advance: {advanceAmount:N2}, Deposit: {depositAmount:N2}.")
            };
                throw new ValidationException(failures);
            }
        }

        // 5) Generate a unique reference number
        var reference = await GenerateUniqueReferenceAsync(ct);

        // 6) Create and persist payment
        var payment = new Payment(
            tenantId: tenantId,
            amount: c.Amount,
            method: c.Method,
            apartmentId: apartmentId,
            referenceNumber: reference,
            notes: c.Notes
        );

        const int maxAttempts = 3;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await _paymentRepo.AddAsync(payment, ct);
                await _paymentRepo.SaveChangesAsync(ct);
                return payment.Id;
            }
            catch (Exception ex) when (IsUniqueRefViolation(ex) && attempt < maxAttempts)
            {
                // regenerate and retry
                reference = PaymentReference.New();
                payment = new Payment(
                    tenantId: tenantId,
                    amount: c.Amount,
                    method: c.Method,
                    apartmentId: apartmentId,
                    referenceNumber: reference,
                    notes: c.Notes
                );
            }
        }
        throw new InvalidOperationException("Failed to persist payment after retries.");
    }
    private async Task<string> GenerateUniqueReferenceAsync(CancellationToken ct)
    {
        for (int i = 0; i < 5; i++)
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
        var typeName = e.GetType().Name;
        if (typeName == "SqlException")
        {
            var number = (int?)e.GetType().GetProperty("Number")?.GetValue(e);
            if (number is 2601 or 2627) return true;
        }
        if (typeName == "PostgresException")
        {
            var state = (string?)e.GetType().GetProperty("SqlState")?.GetValue(e);
            if (state == "23505") return true;
        }
        if (typeName == "SqliteException")
        {
            var code = (int?)e.GetType().GetProperty("SqliteErrorCode")?.GetValue(e);
            if (code == 19) return true;
        }
        return false;
    }
}
