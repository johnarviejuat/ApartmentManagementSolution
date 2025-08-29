using Leasing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Leasing.Infrastructure.Persistence
{
    public sealed class LeasingDbContext(DbContextOptions<LeasingDbContext> options) : DbContext(options) 
    { 
        public DbSet<Lease> Leases => Set<Lease>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("leasing");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LeasingDbContext).Assembly);
        }
    }
}
