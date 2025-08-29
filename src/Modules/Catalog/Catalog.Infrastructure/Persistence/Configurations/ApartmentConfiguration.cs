using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> b)
    {
        b.ToTable("Apartments");

        b.HasKey(a => a.Id);
        b.Property(a => a.Id)
           .HasConversion(id => id.Value, v => new ApartmentId(v))
           .ValueGeneratedNever();

        b.Property(a => a.Name).IsRequired().HasMaxLength(200);
        b.Property(a => a.UnitNumber).IsRequired();

        b.OwnsOne(a => a.Address, a =>
        {
            a.Property(p => p.Line1).HasMaxLength(200).HasColumnName("Address_Line1");
            a.Property(p => p.City).HasMaxLength(100).HasColumnName("Address_City");
            a.Property(p => p.State).HasMaxLength(100).HasColumnName("Address_State");
            a.Property(p => p.PostalCode).HasMaxLength(20).HasColumnName("Address_PostalCode");
        });

        b.Property(a => a.Bedrooms);
        b.Property(a => a.Bathrooms);
        b.Property(a => a.Capacity);
        b.Property(a => a.CurrentCapacity);

        b.Property(a => a.MonthlyRent).HasPrecision(18, 2);
        b.Property(a => a.AdvanceRent).HasPrecision(18, 2);
        b.Property(a => a.SecurityDeposit).HasPrecision(18, 2);

        b.Property(a => a.Status).HasConversion<int>();
        b.Property(a => a.AvailableFrom);
        b.Property(a => a.IsAvailable);
        b.Property(a => a.Description);
        b.Property(a => a.IsDeleted);
        b.Property(a => a.CreatedAt);
        b.Property(a => a.UpdatedAt);
    }
}
