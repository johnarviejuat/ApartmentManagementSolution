using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.History.TenantsHistory;
using ApartmentManagement.Domain.Leasing.Tenants;

namespace ApartmentManagement.Domain.Leasing.Payments
{
    public sealed class Payment
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public TenantId TenantId { get; private set; }
        public ApartmentId? ApartmentId { get; private set; }
        public HistoryId? TenantHistoryId { get; private set; }

        public decimal Amount { get; private set; }
        public DateTime PaidAt { get; private set; }
        public string Method { get; private set; } = default!;
        public string? ReferenceNumber { get; private set; }
        public string? Notes { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        private Payment() { }

        public Payment(
            TenantId tenantId,
            decimal amount,
            string method,
            ApartmentId? apartmentId = null,
            HistoryId? tenantHistoryId = null,
            string? referenceNumber = null,
            string? notes = null)
        {
            TenantId = tenantId;
            ApartmentId = apartmentId;
            TenantHistoryId = tenantHistoryId;
            Amount = amount;
            Method = method;
            ReferenceNumber = referenceNumber;
            Notes = notes;
            PaidAt = DateTime.UtcNow;
        }
    }
}
