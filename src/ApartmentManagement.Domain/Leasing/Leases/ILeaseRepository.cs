using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Tenants;
namespace ApartmentManagement.Domain.Leasing.Leases
{
    public interface ILeaseRepository
    {
        Task<Lease?> GetActiveAsync(TenantId tenantId, ApartmentId apartmentId, CancellationToken ct = default);
        Task AddAsync(Lease lease, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
