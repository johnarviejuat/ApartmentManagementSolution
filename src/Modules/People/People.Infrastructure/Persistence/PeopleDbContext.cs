

using Microsoft.EntityFrameworkCore;
using People.Domain.Entities;
using People.Domain.ValueObjects;

namespace People.Infrastructure.Persistence
{
    public sealed class PeopleDbContext(DbContextOptions<PeopleDbContext> options) : DbContext(options)
    {
        public DbSet<Owner> Owners => Set<Owner>();
        public DbSet<Tenant> Tenants => Set<Tenant>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("people");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PeopleDbContext).Assembly);

            modelBuilder.Owned<Address>();
            modelBuilder.Owned<PersonName>();
            modelBuilder.Owned<Email>();
            modelBuilder.Owned<Phone>();
        }
    }
}
