namespace Billing.Domain.Entities
{
    public interface IAggregateRoot { }
    public sealed class Payment : IAggregateRoot
    {
        private Payment() { }
        public Payment(
            Guid tenantId,
            decimal amount,
            string method,
            Guid apartmentId,
            string? referenceNumber = "",
            string? notes = "")
        {
            TenantId = tenantId;
            ApartmentId = apartmentId;
            Amount = amount;
            Method = method;
            ReferenceNumber = referenceNumber;
            Notes = notes;
            PaidAt = DateTime.UtcNow;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid TenantId { get; private set; }
        public Guid ApartmentId { get; private set; }

        public decimal Amount { get; private set; }
        public DateTime PaidAt { get; private set; }
        public string Method { get; private set; } = default!;
        public string? ReferenceNumber { get; private set; }
        public string? Notes { get; private set; }
        public decimal DepositPortion { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;


        public void MarkDepositPortion(decimal value)
            => DepositPortion = decimal.Round(Math.Max(0, value), 2, MidpointRounding.AwayFromZero);
    }
}
