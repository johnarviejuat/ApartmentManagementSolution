using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Leases;
using ApartmentManagement.Domain.Leasing.Tenants;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Infrastructure.Repositories
{
    public sealed class LeaseRepository(AppDbContext db) : ILeaseRepository
    {
        private readonly AppDbContext _db = db;

        public Task<Lease?> GetActiveAsync(
            TenantId tenantId,
            ApartmentId apartmentId,
            CancellationToken ct = default)
        {
            return _db.Leases
                .FirstOrDefaultAsync(l => l.TenantId == tenantId && l.ApartmentId == apartmentId, ct);
        }
        public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
        public Task AddAsync(Lease lease, CancellationToken ct = default) => _db.Leases.AddAsync(lease, ct).AsTask();
    }
}
