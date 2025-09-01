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

        var leaseIdConverter = new ValueConverter<LeaseId, Guid>(
            id => id.Value,
            guid => new LeaseId(guid));

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasConversion(leaseIdConverter)
            .ValueGeneratedNever();
        b.Property(x => x.TenantId).IsRequired();
        b.Property(x => x.ApartmentId).IsRequired();

        // Money
        b.Property(x => x.MonthlyRent).HasPrecision(18, 2);
        b.Property(x => x.Credit).HasPrecision(18, 2);
        b.Property(x => x.DepositRequired).HasPrecision(18, 2);
        b.Property(x => x.DepositHeld).HasPrecision(18, 2);

        b.Property(x => x.StartDate).IsRequired();
        b.Property(x => x.EndDate);
        b.Property(x => x.NextDueDate).IsRequired();

        b.Ignore(x => x.IsDepositFunded);

        b.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        b.Property(x => x.PreviousLeaseId);

        b.HasOne<Lease>()
            .WithMany()
            .HasForeignKey(x => x.PreviousLeaseId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.PreviousLeaseId)
            .IsUnique()
            .HasFilter("[PreviousLeaseId] IS NOT NULL");

        b.HasIndex(x => new { x.TenantId, x.ApartmentId })
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        b.HasIndex(x => new { x.ApartmentId, x.IsActive });
        b.HasIndex(x => new { x.TenantId, x.IsActive });

        b.ToTable(t => t.HasCheckConstraint("CK_Leases_Deposits_NonNegative",
            "[DepositRequired] >= 0 AND [DepositHeld] >= 0"));

        b.ToTable(t => t.HasCheckConstraint("CK_Leases_Dates_Order",
            "[EndDate] IS NULL OR [EndDate] >= [StartDate]"));
    }
}
