namespace ApartmentManagement.Domain.Leasing.Tenants;

public interface ITenantRepository
{
    Task AddAsync(Tenant tenant, CancellationToken ct = default);
    Task<Tenant?> GetByIdAsync(TenantId id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task UpdateAsync(Tenant tenant, CancellationToken ct = default);
}