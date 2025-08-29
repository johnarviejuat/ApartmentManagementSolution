using People.Domain.Entities;

namespace People.Domain.Abstraction;

public interface ITenantRepository
{
    Task AddAsync(Tenant tenant, CancellationToken ct = default);
    Task<Tenant?> GetByIdAsync(TenantId id, CancellationToken ct = default);
    Task<List<Tenant>> ListByApartmentIdAsync(Guid apartmentId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task UpdateAsync(Tenant tenant, CancellationToken ct = default);
}