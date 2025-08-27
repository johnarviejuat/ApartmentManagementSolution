using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Leases;
using ApartmentManagement.Domain.Leasing.Tenants;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Infrastructure.Repositories
{
    public sealed class LeaseRepository(AppDbContext db) : ILeaseRepository
    {
        private readonly AppDbContext _db = db;

        public Task<Lease?> GetActiveAsync(TenantId tenantId,ApartmentId apartmentId,CancellationToken ct = default)
        {
            return _db.Leases
                .FirstOrDefaultAsync(l => l.TenantId == tenantId && l.ApartmentId == apartmentId && l.IsActive == true, ct);
        }
        public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
        public Task AddAsync(Lease lease, CancellationToken ct = default) => _db.Leases.AddAsync(lease, ct).AsTask();
        public Task<bool> ExistsForTenantApartmentAsync(TenantId tenantId, ApartmentId apartmentId, CancellationToken ct = default) =>
            _db.Leases.AsNoTracking().AnyAsync(p =>
                    p.TenantId == tenantId &&
                    p.ApartmentId == apartmentId &&
                    p.IsActive == true,
                    ct
                );
        public async Task<int> HardDeleteByTenantApartmentAsync(TenantId tenantId, ApartmentId apartmentId, CancellationToken ct = default)
        {
            return await _db.Leases
                .Where(l => l.TenantId == tenantId && l.ApartmentId == apartmentId)
                .ExecuteDeleteAsync(ct);
        }
        public Task<List<Lease>> GetAllAsync(CancellationToken ct = default) =>
            _db.Leases
               .AsNoTracking()
               .ToListAsync(ct);
        public Task<Lease?> GetByIdAsync(Guid id, CancellationToken ct = default) => _db.Leases.FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task<Lease> RenewAsync(
            TenantId tenantId,
            ApartmentId apartmentId,
            DateOnly newStart,
            decimal newRent,
            DateOnly newFirstDue,
            decimal? newDepositRequired,
            CancellationToken ct = default
        )
        {
            var current = await _db.Leases.SingleAsync(
                l => l.TenantId == tenantId && l.ApartmentId == apartmentId && l.IsActive, ct);

            var next = current.Renew(newStart, newRent, newFirstDue, newDepositRequired);

            _db.Leases.Add(next);
            await _db.SaveChangesAsync(ct);
            return next;
        }
    }
}
