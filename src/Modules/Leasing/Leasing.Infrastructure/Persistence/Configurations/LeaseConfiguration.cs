using Catalog.Domain.Entities;
using Leasing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using People.Domain.Entities;

namespace Leasing.Infrastructure.Persistence.Configurations;

public sealed class LeaseConfiguration : IEntityTypeConfiguration<Lease>
{
    public void Configure(EntityTypeBuilder<Lease> b)
    {
        b.ToTable("Leases");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever(); // if you assign Guid yourself

        // 👇 typed IDs are defined in Leasing.Domain.Entities
        b.Property(x => x.TenantId)
         .HasConversion(v => v.ToString(), v => new Guid(v))
         .IsRequired();

        b.Property(x => x.ApartmentId)
         .HasConversion(v => v.ToString(), v => new Guid(v))
         .IsRequired();

        // Money
        b.Property(x => x.MonthlyRent).HasPrecision(18, 2);
        b.Property(x => x.Credit).HasPrecision(18, 2);
        b.Property(x => x.DepositRequired).HasPrecision(18, 2);
        b.Property(x => x.DepositHeld).HasPrecision(18, 2);

        // Dates (EF Core 8 handles DateOnly for SQL Server; if you're on EF7, add a converter)
        b.Property(x => x.NextDueDate); // .HasConversion(dateOnlyConverter) if EF7

        b.Property(x => x.IsActive)
         .HasDefaultValue(true)
         .IsRequired();

        // Active-lease uniqueness per (Tenant, Apartment)
        b.HasIndex(x => new { x.TenantId, x.ApartmentId })
         .IsUnique()
         .HasFilter("[IsActive] = 1");

        b.HasIndex(x => new { x.ApartmentId, x.IsActive });
        b.HasIndex(x => new { x.TenantId, x.IsActive });
    }
}
