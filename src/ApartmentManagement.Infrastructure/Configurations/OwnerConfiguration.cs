using ApartmentManagement.Domain.Leasing.Owners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApartmentManagement.Infrastructure.Configurations;

public sealed class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> b)
    {
        b.ToTable("Owners");

        // Key
        b.HasKey(o => o.Id);
        b.Property(o => o.Id)
            .HasConversion(v => v.Value, v => new OwnerId(v))
            .ValueGeneratedNever();

        // PersonName (owned)
        b.OwnsOne(o => o.Name, nb =>
        {
            nb.Property(p => p.First).HasColumnName("FirstName").HasMaxLength(100).IsRequired();
            nb.Property(p => p.Last).HasColumnName("LastName").HasMaxLength(100).IsRequired();
        });

        // Email (single-value VO -> scalar)
        b.Property(o => o.Email)
            .HasConversion(v => v.Value, v => new Email(v))
            .HasColumnName("Email")
            .HasMaxLength(320)
            .IsRequired();

        // Phone (nullable, single-value VO -> scalar) — use a ValueConverter to handle nulls
        var phoneConverter = new ValueConverter<Phone?, string?>(
            v => v == null ? null : v.Value,
            v => v == null ? null : new Phone(v)
        );

        b.Property(o => o.Phone)
            .HasConversion(phoneConverter)
            .HasColumnName("Phone")
            .HasMaxLength(30);

        // Mailing address (owned, optional)
        b.OwnsOne(o => o.MailingAddress, ab =>
        {
            ab.Property(a => a.Line1).HasColumnName("Mail_Line1").HasMaxLength(200);
            ab.Property(a => a.City).HasColumnName("Mail_City").HasMaxLength(100);
            ab.Property(a => a.State).HasColumnName("Mail_State").HasMaxLength(100);
            ab.Property(a => a.PostalCode).HasColumnName("Mail_PostalCode").HasMaxLength(20);
        });
        b.Navigation(o => o.MailingAddress).IsRequired(false);

        b.Property(o => o.Notes).HasMaxLength(1000);

        b.Property(o => o.IsActive).IsRequired();
        b.Property(o => o.IsDeleted).IsRequired();
        b.Property(o => o.CreatedAt).IsRequired();
        b.Property(o => o.UpdatedAt).IsRequired();

        // Unique Email (works since Email is mapped as scalar)
        b.HasIndex(o => o.Email).IsUnique();
    }
}
