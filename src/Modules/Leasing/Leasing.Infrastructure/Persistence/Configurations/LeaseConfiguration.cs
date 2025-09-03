using Leasing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Leasing.Infrastructure.Persistence.Configurations;

public sealed class LeaseConfiguration : IEntityTypeConfiguration<Lease>
{
    public void Configure(EntityTypeBuilder<Lease> b)
    {
        b.ToTable("Leases");

        // ---- Value converters ----
        var leaseIdConverter = new ValueConverter<LeaseId, Guid>(
            id => id.Value,
            guid => new LeaseId(guid));

        var leaseIdNullableConverter = new ValueConverter<LeaseId?, Guid?>(
            id => id == null ? (Guid?)null : id.Value,
            guid => guid == null ? null : new LeaseId(guid.Value));

        // DateOnly converters (needed for SQL Server on EF Core < 8)
        var dateOnly = new ValueConverter<DateOnly, DateTime>(
            d => d.ToDateTime(TimeOnly.MinValue),
            dt => DateOnly.FromDateTime(dt));

        var dateOnlyNullable = new ValueConverter<DateOnly?, DateTime?>(
            d => d.HasValue ? d.Value.ToDateTime(TimeOnly.MinValue) : null,
            dt => dt.HasValue ? DateOnly.FromDateTime(dt.Value) : null);

        // ---- Key ----
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasConversion(leaseIdConverter)
            .ValueGeneratedNever();

        // ---- FKs / references ----
        b.Property(x => x.TenantId).IsRequired();
        b.Property(x => x.ApartmentId).IsRequired();

        // IMPORTANT: map the nullable value-object FK
        b.Property(x => x.PreviousLeaseId)
            .HasConversion(leaseIdNullableConverter)   // <-- this was missing
            .IsRequired(false);

        // Self-reference (no navs defined on the entity, so use a simple FK)
        b.HasOne<Lease>()
            .WithMany()
            .HasForeignKey(x => x.PreviousLeaseId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---- Money ----
        b.Property(x => x.MonthlyRent).HasPrecision(18, 2);
        b.Property(x => x.Credit).HasPrecision(18, 2);
        b.Property(x => x.DepositRequired).HasPrecision(18, 2);
        b.Property(x => x.DepositHeld).HasPrecision(18, 2);

        // ---- Dates ----
        b.Property(x => x.StartDate).HasConversion(dateOnly).IsRequired();
        b.Property(x => x.NextDueDate).HasConversion(dateOnly).IsRequired();
        b.Property(x => x.EndDate).HasConversion(dateOnlyNullable);

        // ---- Misc ----
        b.Ignore(x => x.IsDepositFunded);

        b.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        // ---- Indexes / constraints ----
        b.HasIndex(x => x.PreviousLeaseId)
            .IsUnique()
            .HasFilter("[PreviousLeaseId] IS NOT NULL");

        b.HasIndex(x => new { x.TenantId, x.ApartmentId })
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        b.HasIndex(x => new { x.ApartmentId, x.IsActive });
        b.HasIndex(x => new { x.TenantId, x.IsActive });

        b.ToTable(t => t.HasCheckConstraint(
            "CK_Leases_Deposits_NonNegative",
            "[DepositRequired] >= 0 AND [DepositHeld] >= 0"));

        b.ToTable(t => t.HasCheckConstraint(
            "CK_Leases_Dates_Order",
            "[EndDate] IS NULL OR [EndDate] >= [StartDate]"));
    }
}
