using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApartmentManagement.Infrastructure.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> b)
    {
        b.ToTable("Tenants");

        // Key (value object)
        b.HasKey(t => t.Id);
        b.Property(t => t.Id)
         .HasConversion(v => v.Value, v => new TenantId(v))
         .ValueGeneratedNever();

        // ---- Converters
        // DateOnly? <-> DateTime?
        var nullableDateOnlyConverter = new ValueConverter<DateOnly?, DateTime?>(
            v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
            v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null);

        // ---- Owned: Name
        b.OwnsOne(t => t.Name, nb =>
        {
            nb.Property(p => p.First).HasMaxLength(100).HasColumnName("FirstName").IsRequired();
            nb.Property(p => p.Last).HasMaxLength(100).HasColumnName("LastName").IsRequired();
        });

        // ---- Owned: Email (unique)
        b.OwnsOne(t => t.Email, nb =>
        {
            nb.Property(p => p.Value)
              .HasMaxLength(320)
              .HasColumnName("Email")
              .IsRequired();

            nb.HasIndex(p => p.Value).IsUnique();
        });

        // ---- Owned: Phone
        b.OwnsOne(t => t.Phone, nb =>
        {
            nb.Property(p => p.Value).HasMaxLength(30).HasColumnName("Phone");
        });

        // ---- Leasing fields
        // If Tenant.ApartmentId is nullable (ApartmentId?):
        b.Property(t => t.ApartmentId)
            .HasConversion(v => v.Value, v => new ApartmentId(v))
            .IsRequired(false);

        b.Property(t => t.MoveInDate).HasConversion(nullableDateOnlyConverter);
        b.Property(t => t.MoveOutDate).HasConversion(nullableDateOnlyConverter);

        // ---- Enum + notes
        b.Property(t => t.Status).HasConversion<int>().IsRequired();
        b.Property(t => t.Notes).HasMaxLength(1000);

        // ---- Audit / soft delete
        b.Property(t => t.IsDeleted).HasDefaultValue(false);
        b.Property(t => t.CreatedAt).IsRequired();
        b.Property(t => t.UpdatedAt).IsRequired();

        // ---- FK to Apartment (no nav required on Tenant)
        b.HasOne<Apartment>()
         .WithMany()
         .HasForeignKey(t => t.ApartmentId)
         .OnDelete(DeleteBehavior.Restrict);

        // ---- Global filter
        b.HasQueryFilter(t => !t.IsDeleted);
    }
}
