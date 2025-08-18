using ApartmentManagement.Domain.Leasing.Apartments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApartmentManagement.Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Apartment> Apartments => Set<Apartment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        var nullableDateOnlyConverter = new ValueConverter<DateOnly?, DateTime?>(
            v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
            v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null);

        b.Entity<Apartment>(cfg =>
        {
            cfg.ToTable("Apartments");

            // Key
            cfg.HasKey(a => a.Id);
            cfg.Property(a => a.Id)
               .HasConversion(v => v.Value, v => new ApartmentId(v))
               .ValueGeneratedNever();

            // Basic fields
            cfg.Property(a => a.Name).HasMaxLength(150).IsRequired();
            cfg.Property(a => a.UnitNumber).IsRequired();
            cfg.Property(a => a.Bedrooms).IsRequired();
            cfg.Property(a => a.Bathrooms).IsRequired();
            cfg.Property(a => a.SquareFeet);

            // Pricing
            cfg.Property(a => a.MonthlyRent)
               .HasColumnType("decimal(18,2)");

            // Availability
            cfg.Property(a => a.AvailableFrom)
               .HasConversion(nullableDateOnlyConverter);  // DateOnly? → DateTime?
            cfg.Property(a => a.IsAvailable).IsRequired();

            // Extras
            cfg.Property(a => a.Description).HasMaxLength(2000);
            cfg.Property(a => a.Amenities).HasConversion<int>(); // flags enum as int

            // Soft delete & audit
            cfg.Property(a => a.IsDeleted).HasDefaultValue(false);
            cfg.Property(a => a.CreatedAt).IsRequired();
            cfg.Property(a => a.UpdatedAt).IsRequired();

            // Owned value object: Address
            cfg.OwnsOne(a => a.Address, nb =>
            {
                nb.Property(p => p.Line1).HasMaxLength(200).HasColumnName("Address_Line1");
                nb.Property(p => p.City).HasMaxLength(100).HasColumnName("Address_City");
                nb.Property(p => p.State).HasMaxLength(50).HasColumnName("Address_State");
                nb.Property(p => p.PostalCode).HasMaxLength(20).HasColumnName("Address_PostalCode");
            });
        });

        // Global query filter for soft delete (optional)
        b.Entity<Apartment>().HasQueryFilter(a => !a.IsDeleted);

        base.OnModelCreating(b);
    }
}
