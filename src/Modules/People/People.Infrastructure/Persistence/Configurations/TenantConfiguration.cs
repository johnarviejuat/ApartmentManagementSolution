using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using People.Domain.Entities;

namespace People.Infrastructure.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> b)
    {
        b.ToTable("Tenants");

        // Key
        b.HasKey(t => t.Id);
        b.Property(t => t.Id)
         .ValueGeneratedNever()
         .HasConversion(v => v.Value, v => new TenantId(v));

        // DateOnly? <-> DateTime?
        var dateOnlyNullable = new ValueConverter<DateOnly?, DateTime?>(
            v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : null,
            v => v.HasValue ? DateOnly.FromDateTime(v.Value) : null);

        // Owned VOs
        b.OwnsOne(t => t.Name, nb =>
        {
            nb.Property(p => p.First).HasMaxLength(100).HasColumnName("FirstName").IsRequired();
            nb.Property(p => p.Last).HasMaxLength(100).HasColumnName("LastName").IsRequired();
        });

        b.OwnsOne(t => t.Email, nb =>
        {
            nb.Property(p => p.Value).HasMaxLength(320).HasColumnName("Email").IsRequired();
            nb.HasIndex(p => p.Value).IsUnique();
        });

        b.OwnsOne(t => t.Phone, nb =>
        {
            nb.Property(p => p.Value).HasMaxLength(30).HasColumnName("Phone");
        });

        // Apartment reference is just an ID (no FK to Catalog)
        b.Property(t => t.ApartmentId).IsRequired(false);

        // Dates
        b.Property(t => t.MoveInDate).HasConversion(dateOnlyNullable);
        b.Property(t => t.MoveOutDate).HasConversion(dateOnlyNullable);

        // Other fields
        b.Property(t => t.Status).HasConversion<int>().IsRequired();
        b.Property(t => t.Notes).HasMaxLength(1000);
        b.Property(t => t.IsDeleted).HasDefaultValue(false);
        b.Property(t => t.CreatedAt).IsRequired();
        b.Property(t => t.UpdatedAt).IsRequired();

        b.HasQueryFilter(t => !t.IsDeleted);
    }
}
