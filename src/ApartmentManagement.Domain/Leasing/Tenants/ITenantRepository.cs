using ApartmentManagement.Domain.Leasing.Apartments;

namespace ApartmentManagement.Domain.Leasing.Tenants;

public interface ITenantRepository
{
    Task AddAsync(Tenant tenant, CancellationToken ct = default);
    Task<Tenant?> GetByIdAsync(TenantId id, CancellationToken ct = default);
    Task<List<Tenant>> ListByApartmentIdAsync(ApartmentId apartmentId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task UpdateAsync(Tenant tenant, CancellationToken ct = default);
}