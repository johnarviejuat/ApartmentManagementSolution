// LeaseConfiguration.cs
using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Leases;
using ApartmentManagement.Domain.Leasing.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Infrastructure.Configurations;

public sealed class LeaseConfiguration : IEntityTypeConfiguration<Lease>
{
    public void Configure(EntityTypeBuilder<Lease> b)
    {
        b.ToTable("Leases");
        b.HasKey(x => x.Id);

        b.Property(x => x.TenantId)
            .HasConversion(v => v.Value, v => new TenantId(v))
            .IsRequired();

        b.Property(x => x.ApartmentId)
            .HasConversion(v => v.Value, v => new ApartmentId(v))
            .IsRequired();

        b.Property(x => x.MonthlyRent).HasColumnType("decimal(18,2)");
        b.Property(x => x.Credit).HasColumnType("decimal(18,2)");
        b.Property(x => x.DepositRequired).HasColumnType("decimal(18,2)");
        b.Property(x => x.DepositHeld).HasColumnType("decimal(18,2)");
        b.Property(x => x.NextDueDate).HasColumnType("date");

        b.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        b.HasIndex(x => new { x.TenantId, x.ApartmentId })
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        b.HasIndex(x => new { x.ApartmentId, x.IsActive });
        b.HasIndex(x => new { x.TenantId, x.IsActive });
    }
}
