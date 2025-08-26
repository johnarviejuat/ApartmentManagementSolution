using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Tenants;

namespace ApartmentManagement.Domain.Leasing.Leases;

public sealed class Lease : IAggregateRoot
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public TenantId TenantId { get; private set; }
    public ApartmentId ApartmentId { get; private set; }
    public decimal MonthlyRent { get; private set; }
    public DateOnly NextDueDate { get; private set; }
    public decimal Credit { get; private set; }
    public decimal DepositRequired { get; private set; }
    public decimal DepositHeld { get; set; }
    public bool IsDepositFunded => DepositHeld >= DepositRequired;


    private Lease() { }

    public Lease(TenantId tenantId, ApartmentId apartmentId, decimal monthlyRent, DateOnly firstDueDate, decimal depositRequired)
    {
        TenantId = tenantId;
        ApartmentId = apartmentId;
        MonthlyRent = monthlyRent;
        NextDueDate = firstDueDate;
        DepositRequired = depositRequired;
        DepositHeld = 0m;
        Credit = 0m;
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
}
