using ApartmentManagement.Domain.Leasing.Owners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Infrastructure.Configurations;

public sealed class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> b)
    {
        // Table
        b.ToTable("Owners");

        // Key (value object)
        b.HasKey(o => o.Id);
        b.Property(o => o.Id)
            .HasConversion(v => v.Value, v => new OwnerId(v))
            .ValueGeneratedNever();

        // Owned: Name (First/Last)
        b.OwnsOne(o => o.Name, nb =>
        {
            nb.Property(p => p.First).HasMaxLength(100).HasColumnName("FirstName").IsRequired();
            nb.Property(p => p.Last).HasMaxLength(100).HasColumnName("LastName").IsRequired();
        });

        // Owned: Email (Value) with unique index
        b.OwnsOne(o => o.Email, nb =>
        {
            nb.Property(p => p.Value)
              .HasMaxLength(320)
              .HasColumnName("Email")
              .IsRequired();

            // Unique email per owner
            nb.HasIndex(p => p.Value).IsUnique();
        });

        // Owned: Phone (Value)
        b.OwnsOne(o => o.Phone, nb =>
        {
            nb.Property(p => p.Value)
              .HasMaxLength(30)
              .HasColumnName("Phone");
        });

        // Owned: MailingAddress
        b.OwnsOne(o => o.MailingAddress, nb =>
        {
            nb.Property(p => p.Line1).HasMaxLength(200).HasColumnName("Mail_Line1");
            nb.Property(p => p.City).HasMaxLength(100).HasColumnName("Mail_City");
            nb.Property(p => p.State).HasMaxLength(50).HasColumnName("Mail_State");
            nb.Property(p => p.PostalCode).HasMaxLength(20).HasColumnName("Mail_PostalCode");
        });

        // Scalars
        b.Property(o => o.Notes).HasMaxLength(1000);
        b.Property(o => o.IsActive).IsRequired();
        b.Property(o => o.IsDeleted).IsRequired();
        b.Property(o => o.CreatedAt).IsRequired();
        b.Property(o => o.UpdatedAt).IsRequired();

        // Optional global filter (only if you want soft-delete behavior here too)
        // b.HasQueryFilter(o => !o.IsDeleted);
    }
}
