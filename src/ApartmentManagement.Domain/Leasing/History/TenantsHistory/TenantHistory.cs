using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Tenants;

namespace ApartmentManagement.Domain.Leasing.History.TenantsHistory
{
    public sealed record HistoryId(Guid Value);
    public sealed record TenantName(string First, string Last)
    {
        public override string ToString() => $"{First} {Last}".Trim();
    }
    public sealed record TenantEmail(string Value)
    {
        public override string ToString() => Value;
    }
    public sealed record TenantPhone(string Value)
    {
        public override string ToString() => Value;
    }

    public sealed class TenantHistory : IAggregateRoot
    {
        public HistoryId Guid { get; private set; } = default!;

        // Identity
        public TenantId TenantId { get; set; } = default!;
        public TenantName Name { get; set; } = default!;
        public TenantEmail Email { get; set; } = default!;
        public TenantPhone? Phone { get; set; }

        // Leasing details
        public ApartmentId? ApartmentId { get; set; }
        public DateOnly? MoveInDate { get; set; }
        public DateOnly? MoveOutDate { get; set; }
        public decimal? RentAmount { get; set; } // Track rent amount during lease
        public decimal? SecurityDeposit { get; set; } // Track security deposit
        public string? LeaseTerms { get; set; } // Track specific lease terms
        public bool IsRenewed { get; set; } // Whether the lease was renewed
        public DateTime? LeaseRenewalDate { get; set; } // Date when the lease was renewed

        // Additional tenant changes/events
        public bool HasEarlyTermination { get; set; } // Flag for early termination
        public bool WasEvicted { get; set; } // Flag for eviction
        public string? EvictionReason { get; set; } // Reason for eviction (if applicable)
        public string? Notes { get; set; }
        public int Status { get; set; }
        public bool IsDeleted { get; set; }

        // Important Dates
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        // Additional history information (e.g., Move-in/Move-out inspection)
        public string? InspectionReport { get; set; } // Link to or text description of the inspection report
        public string? ApartmentConditionAtMoveIn { get; set; } // Notes on apartment condition at move-in
        public string? ApartmentConditionAtMoveOut { get; set; } // Notes on apartment condition at move-out
    }

}
