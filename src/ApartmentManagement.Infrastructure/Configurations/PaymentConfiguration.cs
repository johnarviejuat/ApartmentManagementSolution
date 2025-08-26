using ApartmentManagement.Domain.Leasing.Payments;
using ApartmentManagement.Domain.Leasing.Tenants;
using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.History.TenantsHistory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Infrastructure.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.ToTable("Payments");

        b.HasKey(p => p.Id);
        // Id is a Guid generated in the domain ctor; no special config needed.

        // ----- Value-object FKs (store as Guid)
        b.Property(p => p.TenantId)
         .HasConversion(v => v.Value, v => new TenantId(v))
         .IsRequired();

        b.Property(p => p.ApartmentId)
         .HasConversion(v => v.Value, v => new ApartmentId(v))
         .IsRequired();

        b.Property(p => p.TenantHistoryId)
         .HasConversion(v => v.Value, v => new HistoryId(v))
         .IsRequired(false);

        // ----- Scalars
        b.Property(p => p.Amount)
         .HasColumnType("decimal(18,2)")
         .IsRequired();

        b.Property(p => p.Method)
         .HasMaxLength(50)
         .IsRequired();

        b.Property(p => p.ReferenceNumber)
         .HasMaxLength(50)
         .IsRequired();

        b.HasIndex(p => p.ReferenceNumber).IsUnique();

        b.Property(p => p.Notes)
         .HasMaxLength(1000);

        b.Property(p => p.DepositPortion)
         .HasColumnType("decimal(18,2)")
         .HasDefaultValue(0m);

        b.Property(p => p.PaidAt).IsRequired();
        b.Property(p => p.CreatedAt).IsRequired();
        b.Property(p => p.UpdatedAt).IsRequired();

        // ----- Relationships (no navs on Payment required)
        b.HasOne<Tenant>()
         .WithMany()
         .HasForeignKey(p => p.TenantId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<Apartment>()
         .WithMany()
         .HasForeignKey(p => p.ApartmentId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<TenantHistory>()
         .WithMany()
         .HasForeignKey(p => p.TenantHistoryId)
         .OnDelete(DeleteBehavior.SetNull);
    }
}
