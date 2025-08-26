using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Owners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApartmentManagement.Infrastructure.Configurations;

public sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> b)
    {
        // Table
        b.ToTable("Apartments");

        // Key (value object)
        b.HasKey(a => a.Id);
        b.Property(a => a.Id)
            .HasConversion(v => v.Value, v => new ApartmentId(v))
            .ValueGeneratedNever();

        // Nullable DateOnly converter (EF Core < 7 needs this; keep it for safety)
        var nullableDateOnlyConverter = new ValueConverter<DateOnly?, DateTime?>(
            v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
            v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null);

        // Scalars
        b.Property(a => a.Name).HasMaxLength(150).IsRequired();
        b.Property(a => a.UnitNumber).IsRequired();
        b.Property(a => a.Bedrooms).IsRequired();
        b.Property(a => a.Bathrooms).IsRequired();
        b.Property(a => a.SquareFeet);

        b.Property(a => a.MonthlyRent).HasColumnType("decimal(18,2)");
        b.Property(a => a.AdvanceRent).HasColumnType("decimal(18,2)");
        b.Property(a => a.SecurityDeposit).HasColumnType("decimal(18,2)");

        b.Property(a => a.AvailableFrom).HasConversion(nullableDateOnlyConverter);
        b.Property(a => a.IsAvailable).IsRequired();
        b.Property(a => a.Description).HasMaxLength(2000);

        b.Property(a => a.Status).HasConversion<int>();

        // Owner FK (optional)
        b.Property(a => a.OwnerId)
            .HasConversion(v => v.Value, v => new OwnerId(v))
            .IsRequired(false);

        b.Property(a => a.OwnershipAssignedAt);

        b.HasOne<Owner>()                 // assuming Owner is an entity
            .WithMany()
            .HasForeignKey(a => a.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Auditing / soft delete
        b.Property(a => a.IsDeleted).HasDefaultValue(false);
        b.Property(a => a.CreatedAt).IsRequired();
        b.Property(a => a.UpdatedAt).IsRequired();

        // Owned type: Address
        b.OwnsOne(a => a.Address, nb =>
        {
            nb.Property(p => p.Line1).HasMaxLength(200).HasColumnName("Address_Line1");
            nb.Property(p => p.City).HasMaxLength(100).HasColumnName("Address_City");
            nb.Property(p => p.State).HasMaxLength(50).HasColumnName("Address_State");
            nb.Property(p => p.PostalCode).HasMaxLength(20).HasColumnName("Address_PostalCode");
        });

        // Global query filter (soft-delete)
        b.HasQueryFilter(a => !a.IsDeleted);
    }
}
