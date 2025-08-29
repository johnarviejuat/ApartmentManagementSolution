
namespace Leasing.Domain.Entities;

public interface IAggregateRoot { }
public sealed class Lease : IAggregateRoot
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TenantId { get; private set; }
    public Guid ApartmentId { get; private set; }

    // Per-term economics
    public decimal MonthlyRent { get; private set; }
    public DateOnly NextDueDate { get; private set; }
    public decimal Credit { get; private set; }
    public decimal DepositRequired { get; private set; }
    public decimal DepositHeld { get; set; }
    public bool IsDepositFunded => DepositHeld >= DepositRequired;

    // Term boundaries + lifecycle
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid? PreviousLeaseId { get; private set; }

    private Lease() { }

    public Lease(
        Guid tenantId,
        Guid apartmentId,
        decimal monthlyRent,
        DateOnly firstDueDate,
        decimal depositRequired,
        DateOnly startDate)
    {
        TenantId = tenantId;
        ApartmentId = apartmentId;
        MonthlyRent = monthlyRent;
        NextDueDate = firstDueDate;
        DepositRequired = depositRequired;
        DepositHeld = 0m;
        Credit = 0m;

        StartDate = startDate;
        EndDate = null;
        IsActive = true;
    }

    public Lease Renew(
        DateOnly newStartDate,
        decimal newMonthlyRent,
        DateOnly newFirstDueDate,
        decimal? newDepositRequired = null,
        bool carryOverCredit = true,
        bool carryOverDeposit = true
    )
    {
        if (!IsActive) throw new InvalidOperationException("Cannot renew an inactive lease.");
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(newStartDate, StartDate);
        IsActive = false;
        EndDate = newStartDate.AddDays(-1);

        var next = new Lease(
            TenantId,
            ApartmentId,
            newMonthlyRent,
            newFirstDueDate,
            newDepositRequired ?? DepositRequired,
            newStartDate)
        {
            PreviousLeaseId = Id,
            Credit = carryOverCredit ? Credit : 0m,
            DepositHeld = carryOverDeposit ? DepositHeld : 0m
        };

        return next;
    }

    public void FundDeposit(decimal amount)
    {
        DepositHeld = decimal.Round(DepositHeld + amount, 2, MidpointRounding.AwayFromZero);
    }
    public (int monthsCovered, decimal remainder) ApplyPayment(decimal amount)
    {
        amount = decimal.Round(amount + Credit, 2, MidpointRounding.AwayFromZero);

        var months = 0;
        while (amount + 0.0001m >= MonthlyRent)
        {
            amount -= MonthlyRent;
            months++;
            NextDueDate = NextDueDate.AddMonths(1);
        }

        Credit = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        return (months, Credit);
    }
    public void Terminate()
    {
        if (!IsActive) return;
        IsActive = false;
        EndDate = DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
