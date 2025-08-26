using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.History.TenantsHistory;
using ApartmentManagement.Domain.Leasing.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApartmentManagement.Infrastructure.Configurations;

public sealed class TenantHistoryConfiguration : IEntityTypeConfiguration<TenantHistory>
{
    public void Configure(EntityTypeBuilder<TenantHistory> b)
    {
        b.ToTable("TenantHistories");

        // PK (property is named Guid in your domain; store as column "Id")
        b.HasKey(h => h.Guid);
        b.Property(h => h.Guid)
         .HasColumnName("Id")
         .HasConversion(v => v.Value, v => new HistoryId(v))
         .ValueGeneratedNever();

        // VO conversions
        b.Property(h => h.TenantId)
         .HasConversion(v => v.Value, v => new TenantId(v))
         .IsRequired();

        // nullable ApartmentId
        var nullableApartmentId = new ValueConverter<ApartmentId?, Guid?>(
            v => v == null ? (Guid?)null : v.Value,
            v => v.HasValue ? new ApartmentId(v.Value) : null
        );

        b.Property(h => h.ApartmentId)
         .HasConversion(nullableApartmentId)
         .IsRequired(false);

        // DateOnly? converter
        var nullableDateOnly = new ValueConverter<DateOnly?, DateTime?>(
            v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
            v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null
        );
        b.Property(h => h.MoveInDate).HasConversion(nullableDateOnly);
        b.Property(h => h.MoveOutDate).HasConversion(nullableDateOnly);

        // Scalars
        b.Property(h => h.RentAmount).HasColumnType("decimal(18,2)");
        b.Property(h => h.SecurityDeposit).HasColumnType("decimal(18,2)");
        b.Property(h => h.LeaseTerms).HasMaxLength(2000);
        b.Property(h => h.IsRenewed);
        b.Property(h => h.LeaseRenewalDate);
        b.Property(h => h.HasEarlyTermination);
        b.Property(h => h.WasEvicted);
        b.Property(h => h.EvictionReason).HasMaxLength(1000);
        b.Property(h => h.Notes).HasMaxLength(2000);
        b.Property(h => h.Status).IsRequired();
        b.Property(h => h.IsDeleted).HasDefaultValue(false);
        b.Property(h => h.CreatedAt).IsRequired();
        b.Property(h => h.UpdatedAt).IsRequired();

        // Owned value objects
        b.OwnsOne(h => h.Name, nb =>
        {
            nb.Property(p => p.First).HasMaxLength(100).HasColumnName("FirstName").IsRequired();
            nb.Property(p => p.Last).HasMaxLength(100).HasColumnName("LastName").IsRequired();
        });

        b.OwnsOne(h => h.Email, nb =>
        {
            nb.Property(p => p.Value).HasMaxLength(320).HasColumnName("Email").IsRequired();
            nb.HasIndex(p => p.Value).IsUnique(false);
        });

        b.OwnsOne(h => h.Phone, nb =>
        {
            nb.Property(p => p.Value).HasMaxLength(30).HasColumnName("Phone");
        });

        // Relationships
        b.HasOne<Apartment>()
         .WithMany()
         .HasForeignKey(h => h.ApartmentId)
         .OnDelete(DeleteBehavior.Restrict);

        // Helpful indexes
        b.HasIndex(h => new { h.TenantId, h.CreatedAt });
        b.HasIndex(h => h.ApartmentId);
    }
}
