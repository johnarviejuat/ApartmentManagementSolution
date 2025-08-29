using Billing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Infrastructure.Persistence.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.ToTable("Payments");

        b.HasKey(p => p.Id);

        b.Property(p => p.TenantId)
         .HasConversion(v => v.ToString(), v => new Guid(v))
         .IsRequired();

        b.Property(p => p.ApartmentId)
         .HasConversion(v => v.ToString(), v => new Guid(v))
         .IsRequired();

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
    }
}
