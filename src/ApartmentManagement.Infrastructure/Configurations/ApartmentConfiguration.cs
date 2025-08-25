using ApartmentManagement.Domain.Leasing.Apartments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> b)
    {
        b.ToTable("Apartments");
        b.HasKey(a => a.Id);
        b.Property(a => a.Id)
            .HasConversion(id => id.Value, v => new ApartmentId(v))
            .ValueGeneratedNever();

        b.Property(a => a.Name).HasMaxLength(150).IsRequired();
        b.Property(a => a.UnitNumber).IsRequired();
        b.Property(a => a.Bedrooms).IsRequired();
        b.Property(a => a.Bathrooms).IsRequired();
        b.Property(a => a.SquareFeet);
        b.Property(a => a.MonthlyRent).HasColumnType("decimal(18,2)");
        b.Property(a => a.AdvanceRent).HasColumnType("decimal(18,2)");
        b.Property(a => a.SecurityDeposit).HasColumnType("decimal(18,2)");
        b.Property(a => a.Description).HasMaxLength(2000);
        b.Property(a => a.IsDeleted).HasDefaultValue(false);

        b.OwnsOne(a => a.Address, addr =>
        {
            addr.Property(p => p.Line1).HasMaxLength(200).HasColumnName("Address_Line1");
            addr.Property(p => p.City).HasMaxLength(100).HasColumnName("Address_City");
            addr.Property(p => p.State).HasMaxLength(50).HasColumnName("Address_State");
            addr.Property(p => p.PostalCode).HasMaxLength(20).HasColumnName("Address_PostalCode");
        });
    }
}
