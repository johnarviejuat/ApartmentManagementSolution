using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using People.Domain.Entities;

namespace People.Infrastructure.Persistence.Configurations;

public sealed class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> b)
    {
        b.ToTable("Owners");

        b.HasKey(o => o.Id);
        b.Property(o => o.Id)
         .HasConversion(v => v.Value, v => new OwnerId(v))
         .ValueGeneratedNever();

        b.OwnsOne(o => o.Name, nb =>
        {
            nb.Property(p => p.First).HasMaxLength(100).HasColumnName("FirstName").IsRequired();
            nb.Property(p => p.Last).HasMaxLength(100).HasColumnName("LastName").IsRequired();
        });

        b.OwnsOne(o => o.Email, nb =>
        {
            nb.Property(p => p.Value).HasMaxLength(320).HasColumnName("Email").IsRequired();
            nb.HasIndex(p => p.Value).IsUnique();
        });

        b.OwnsOne(o => o.Phone, nb =>
        {
            nb.Property(p => p.Value).HasMaxLength(30).HasColumnName("Phone");
        });

        b.OwnsOne(o => o.MailingAddress, a =>
        {
            a.Property(p => p.Line1).HasMaxLength(200).HasColumnName("Mail_Line1");
            a.Property(p => p.City).HasMaxLength(100).HasColumnName("Mail_City");
            a.Property(p => p.State).HasMaxLength(100).HasColumnName("Mail_State");
            a.Property(p => p.PostalCode).HasMaxLength(20).HasColumnName("Mail_PostalCode");
        });

        b.Property(o => o.Notes).HasMaxLength(1000);
        b.Property(o => o.IsActive).IsRequired();
        b.Property(o => o.IsDeleted).IsRequired();
        b.Property(o => o.CreatedAt).IsRequired();
        b.Property(o => o.UpdatedAt).IsRequired();
    }
}
