using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Tenants;
namespace ApartmentManagement.Domain.Leasing.Leases
{
    public interface ILeaseRepository
    {
        Task<Lease?> GetActiveAsync(TenantId tenantId, ApartmentId apartmentId, CancellationToken ct = default);
        Task AddAsync(Lease lease, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
        Task<bool> ExistsForTenantApartmentAsync(TenantId tenantId, ApartmentId apartmentId, CancellationToken ct = default);
        Task<int> HardDeleteByTenantApartmentAsync(TenantId tenantId, ApartmentId apartmentId, CancellationToken ct = default);
        Task<List<Lease>> GetAllAsync(CancellationToken ct = default);
        Task<Lease?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Lease> RenewAsync(
            TenantId tenantId,
            ApartmentId apartmentId,
            DateOnly newStart,
            decimal newRent,
            DateOnly newFirstDue,
            decimal? newDepositRequired,
            CancellationToken ct = default
        );
    }
}
