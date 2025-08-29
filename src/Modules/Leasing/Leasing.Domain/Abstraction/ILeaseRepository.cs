

using Leasing.Domain.Entities;

namespace Leasing.Domain.Abstraction
{
    public interface ILeaseRepository
    {
        Task<Lease?> GetActiveAsync(Guid tenantId, Guid apartmentId, CancellationToken ct = default);
        Task AddAsync(Lease lease, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
        Task<bool> ExistsForTenantApartmentAsync(Guid tenantId, Guid apartmentId, CancellationToken ct = default);
        Task<int> HardDeleteByTenantApartmentAsync(Guid tenantId, Guid apartmentId, CancellationToken ct = default);
        Task<List<Lease>> GetAllAsync(CancellationToken ct = default);
        Task<Lease?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Lease> RenewAsync(
            Guid tenantId,
            Guid apartmentId,
            DateOnly newStart,
            decimal newRent,
            DateOnly newFirstDue,
            decimal? newDepositRequired,
            CancellationToken ct = default
        );
    }
}
