using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.History.TenantsHistory;
using ApartmentManagement.Domain.Leasing.Owners;
using ApartmentManagement.Domain.Leasing.Payments;
using ApartmentManagement.Domain.Leasing.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApartmentManagement.Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Apartment> Apartments => Set<Apartment>();
    public DbSet<Owner> Owners => Set<Owner>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantHistory> TenantHistories { get; set; }
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Converter for nullable DateOnly
        var nullableDateOnlyConverter = new ValueConverter<DateOnly?, DateTime?>(
            v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
            v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null);

        // ===== Apartment =====
        b.Entity<Apartment>(cfg =>
        {
            cfg.ToTable("Apartments");

            cfg.HasKey(a => a.Id);
            cfg.Property(a => a.Id)
               .HasConversion(v => v.Value, v => new ApartmentId(v))
               .ValueGeneratedNever();

            cfg.Property(a => a.Name).HasMaxLength(150).IsRequired();
            cfg.Property(a => a.UnitNumber).IsRequired();
            cfg.Property(a => a.Bedrooms).IsRequired();
            cfg.Property(a => a.Bathrooms).IsRequired();
            cfg.Property(a => a.SquareFeet);
            cfg.Property(a => a.MonthlyRent).HasColumnType("decimal(18,2)");
            cfg.Property(a => a.AdvanceRent).HasColumnType("decimal(18,2)");
            cfg.Property(a => a.SecurityDeposit).HasColumnType("decimal(18,2)");
            cfg.Property(a => a.AvailableFrom).HasConversion(nullableDateOnlyConverter);
            cfg.Property(a => a.IsAvailable).IsRequired();
            cfg.Property(a => a.Description).HasMaxLength(2000);
            cfg.Property(a => a.Status).HasConversion<int>();

            // Ownership FK
            cfg.Property(a => a.OwnerId)
               .HasConversion(v => v.Value, v => new OwnerId(v))
               .IsRequired(false);
            cfg.Property(a => a.OwnershipAssignedAt);
            cfg.HasOne<Owner>()
               .WithMany()
               .HasForeignKey(a => a.OwnerId)
               .OnDelete(DeleteBehavior.Restrict);

            cfg.Property(a => a.IsDeleted).HasDefaultValue(false);
            cfg.Property(a => a.CreatedAt).IsRequired();
            cfg.Property(a => a.UpdatedAt).IsRequired();

            cfg.OwnsOne(a => a.Address, nb =>
            {
                nb.Property(p => p.Line1).HasMaxLength(200).HasColumnName("Address_Line1");
                nb.Property(p => p.City).HasMaxLength(100).HasColumnName("Address_City");
                nb.Property(p => p.State).HasMaxLength(50).HasColumnName("Address_State");
                nb.Property(p => p.PostalCode).HasMaxLength(20).HasColumnName("Address_PostalCode");
            });
        });
        b.Entity<Apartment>().HasQueryFilter(a => !a.IsDeleted);

        // ===== Owner =====
        b.Entity<Owner>(cfg =>
        {
            cfg.ToTable("Owners");

            cfg.HasKey(o => o.Id);
            cfg.Property(o => o.Id)
               .HasConversion(v => v.Value, v => new OwnerId(v))
               .ValueGeneratedNever();

            cfg.OwnsOne(o => o.Name, nb =>
            {
                nb.Property(p => p.First).HasMaxLength(100).HasColumnName("FirstName").IsRequired();
                nb.Property(p => p.Last).HasMaxLength(100).HasColumnName("LastName").IsRequired();
            });

            cfg.OwnsOne(o => o.Email, nb =>
            {
                nb.Property(p => p.Value)
                  .HasMaxLength(320)
                  .HasColumnName("Email")
                  .IsRequired();

                // unique email per owner
                nb.HasIndex(p => p.Value).IsUnique();
            });

            cfg.OwnsOne(o => o.Phone, nb =>
            {
                nb.Property(p => p.Value).HasMaxLength(30).HasColumnName("Phone");
            });

            cfg.OwnsOne(o => o.MailingAddress, nb =>
            {
                nb.Property(p => p.Line1).HasMaxLength(200).HasColumnName("Mail_Line1");
                nb.Property(p => p.City).HasMaxLength(100).HasColumnName("Mail_City");
                nb.Property(p => p.State).HasMaxLength(50).HasColumnName("Mail_State");
                nb.Property(p => p.PostalCode).HasMaxLength(20).HasColumnName("Mail_PostalCode");
            });

            cfg.Property(o => o.Notes).HasMaxLength(1000);
            cfg.Property(o => o.IsActive).IsRequired();
            cfg.Property(o => o.IsDeleted).IsRequired();
            cfg.Property(o => o.CreatedAt).IsRequired();
            cfg.Property(o => o.UpdatedAt).IsRequired();
        });

        // ===== Tenant =====
        b.Entity<Tenant>(cfg =>
        {
            cfg.ToTable("Tenants");

            cfg.HasKey(t => t.Id);
            cfg.Property(t => t.Id)
               .HasConversion(v => v.Value, v => new TenantId(v))
               .ValueGeneratedNever();

            // Name (owned)
            cfg.OwnsOne(t => t.Name, nb =>
            {
                nb.Property(p => p.First).HasMaxLength(100).HasColumnName("FirstName").IsRequired();
                nb.Property(p => p.Last).HasMaxLength(100).HasColumnName("LastName").IsRequired();
            });

            // Email (owned) + unique
            cfg.OwnsOne(t => t.Email, nb =>
            {
                nb.Property(p => p.Value)
                  .HasMaxLength(320)
                  .HasColumnName("Email")
                  .IsRequired();

                nb.HasIndex(p => p.Value).IsUnique();
            });

            // Phone (owned)
            cfg.OwnsOne(t => t.Phone, nb =>
            {
                nb.Property(p => p.Value).HasMaxLength(30).HasColumnName("Phone");
            });

            // Leasing fields
            cfg.Property(t => t.ApartmentId)
               .HasConversion(v => v.Value, v => new ApartmentId(v))
               .IsRequired(false);

            cfg.Property(t => t.MoveInDate).HasConversion(nullableDateOnlyConverter);
            cfg.Property(t => t.MoveOutDate).HasConversion(nullableDateOnlyConverter);

            // Enum + notes
            cfg.Property(t => t.Status).HasConversion<int>().IsRequired();
            cfg.Property(t => t.Notes).HasMaxLength(1000);

            // Audit/soft delete
            cfg.Property(t => t.IsDeleted).HasDefaultValue(false);
            cfg.Property(t => t.CreatedAt).IsRequired();
            cfg.Property(t => t.UpdatedAt).IsRequired();

            // FK to Apartment (no navs required)
            cfg.HasOne<Apartment>()
               .WithMany()
               .HasForeignKey(t => t.ApartmentId)
               .OnDelete(DeleteBehavior.Restrict);
        });
        b.Entity<Tenant>().HasQueryFilter(t => !t.IsDeleted);

        // ===== TenantHistory =====
        b.Entity<TenantHistory>(cfg =>
        {
            cfg.ToTable("TenantHistories");

            // Primary key (explicit because property is named 'Guid' and is a value object)
            cfg.HasKey(h => h.Guid);
            cfg.Property(h => h.Guid)
               .HasColumnName("Id")
               .HasConversion(v => v.Value, v => new HistoryId(v))
               .ValueGeneratedNever();

            // Identity (value objects)
            cfg.Property(h => h.TenantId).IsRequired();

            cfg.OwnsOne(h => h.Name, nb =>
            {
                nb.Property(p => p.First).HasMaxLength(100).HasColumnName("FirstName").IsRequired();
                nb.Property(p => p.Last).HasMaxLength(100).HasColumnName("LastName").IsRequired();
            });

            cfg.OwnsOne(h => h.Email, nb =>
            {
                nb.Property(p => p.Value).HasMaxLength(320).HasColumnName("Email").IsRequired();
                nb.HasIndex(p => p.Value).IsUnique(false); // history may repeat same email
            });

            cfg.OwnsOne(h => h.Phone, nb =>
            {
                nb.Property(p => p.Value).HasMaxLength(30).HasColumnName("Phone");
            });

            // Leasing details
            cfg.Property(h => h.ApartmentId)
               .HasConversion(v => v == null ? (Guid?)null : v.Value,
                              v => v.HasValue ? new ApartmentId(v.Value) : null)
               .IsRequired(false);

            cfg.Property(h => h.MoveInDate).HasConversion(nullableDateOnlyConverter);
            cfg.Property(h => h.MoveOutDate).HasConversion(nullableDateOnlyConverter);
            cfg.Property(h => h.RentAmount).HasColumnType("decimal(18,2)");
            cfg.Property(h => h.SecurityDeposit).HasColumnType("decimal(18,2)");
            cfg.Property(h => h.LeaseTerms).HasMaxLength(2000);
            cfg.Property(h => h.IsRenewed);
            cfg.Property(h => h.LeaseRenewalDate);

            // Events / flags
            cfg.Property(h => h.HasEarlyTermination);
            cfg.Property(h => h.WasEvicted);
            cfg.Property(h => h.EvictionReason).HasMaxLength(1000);
            cfg.Property(h => h.Notes).HasMaxLength(2000);
            cfg.Property(h => h.Status).IsRequired();
            cfg.Property(h => h.IsDeleted).HasDefaultValue(false);

            // Audit
            cfg.Property(h => h.CreatedAt).IsRequired();
            cfg.Property(h => h.UpdatedAt).IsRequired();

            // Extra history info
            cfg.Property(h => h.InspectionReport).HasMaxLength(4000);
            cfg.Property(h => h.ApartmentConditionAtMoveIn).HasMaxLength(2000);
            cfg.Property(h => h.ApartmentConditionAtMoveOut).HasMaxLength(2000);

            // Relationships
            cfg.HasOne<Apartment>()
               .WithMany()
               .HasForeignKey(h => h.ApartmentId)
               .OnDelete(DeleteBehavior.Restrict);

            // Helpful indexes for querying history
            cfg.HasIndex(h => new { h.TenantId, h.CreatedAt });
            cfg.HasIndex(h => h.ApartmentId);
        });

        // ===== Payment =====
        b.Entity<Payment>(cfg =>
        {
            cfg.ToTable("Payments");

            cfg.HasKey(p => p.Id);

            cfg.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            cfg.Property(p => p.Method)
                .HasMaxLength(50)
                .IsRequired();

            cfg.Property(p => p.ReferenceNumber)
               .HasMaxLength(50)
               .IsRequired();
            cfg.HasIndex(p => p.ReferenceNumber).IsUnique();

            cfg.Property(p => p.Notes)
                .HasMaxLength(1000);

            cfg.Property(p => p.PaidAt).IsRequired();
            cfg.Property(p => p.CreatedAt).IsRequired();
            cfg.Property(p => p.UpdatedAt).IsRequired();

            // Relationships (optional navigation if you want)
            cfg.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(p => p.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            cfg.HasOne<Apartment>()
                .WithMany()
                .HasForeignKey(p => p.ApartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            cfg.HasOne<TenantHistory>()
                .WithMany()
                .HasForeignKey(p => p.TenantHistoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        base.OnModelCreating(b);
    }
}
