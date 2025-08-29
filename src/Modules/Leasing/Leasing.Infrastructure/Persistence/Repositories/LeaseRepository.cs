using Leasing.Domain.Abstraction;
using Leasing.Domain.Entities;  
using Microsoft.EntityFrameworkCore;

namespace Leasing.Infrastructure.Persistence.Repositories
{
    public sealed class LeaseRepository(LeasingDbContext db) : ILeaseRepository
    {
        private readonly LeasingDbContext _db = db;

        public Task<Lease?> GetActiveAsync(Guid tenantId, Guid apartmentId, CancellationToken ct = default) =>
            _db.Leases
               .AsNoTracking()
               .FirstOrDefaultAsync(l =>
                    l.TenantId == tenantId &&
                    l.ApartmentId == apartmentId &&
                    l.IsActive, ct);

        public Task<bool> ExistsForTenantApartmentAsync(Guid tenantId, Guid apartmentId, CancellationToken ct = default) =>
            _db.Leases
               .AsNoTracking()
               .AnyAsync(l =>
                    l.TenantId == tenantId &&
                    l.ApartmentId == apartmentId &&
                    l.IsActive, ct);

        public Task<List<Lease>> GetAllAsync(CancellationToken ct = default) =>
            _db.Leases.AsNoTracking().ToListAsync(ct);

        public Task<Lease?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            _db.Leases.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task AddAsync(Lease lease, CancellationToken ct = default) =>
            _db.Leases.AddAsync(lease, ct).AsTask();

        public Task SaveChangesAsync(CancellationToken ct = default) =>
            _db.SaveChangesAsync(ct);

        public Task<int> HardDeleteByTenantApartmentAsync(Guid tenantId, Guid apartmentId, CancellationToken ct = default) =>
            _db.Leases
               .Where(l => l.TenantId == tenantId && l.ApartmentId == apartmentId)
               .ExecuteDeleteAsync(ct);

        public async Task<Lease> RenewAsync(
            Guid tenantId,
            Guid apartmentId,
            DateOnly newStart,
            decimal newRent,
            DateOnly newFirstDue,
            decimal? newDepositRequired,
            CancellationToken ct = default)
        {
            var current = await _db.Leases.SingleAsync(
                l => l.TenantId == tenantId &&
                     l.ApartmentId == apartmentId &&
                     l.IsActive, ct);

            var next = current.Renew(newStart, newRent, newFirstDue, newDepositRequired);

            _db.Leases.Add(next);
            await _db.SaveChangesAsync(ct);
            return next;
        }
    }
}
