using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Tenants;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Infrastructure.Repositories;

public sealed class TenantRepository(AppDbContext db) : ITenantRepository
{
    private readonly AppDbContext _db = db;

    public Task AddAsync(Tenant tenant, CancellationToken ct = default)
        => _db.Tenants.AddAsync(tenant, ct).AsTask();

    public Task<Tenant?> GetByIdAsync(TenantId id, CancellationToken ct = default)
        => _db.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct)!;

    public Task<List<Tenant>> ListByApartmentIdAsync(ApartmentId apartmentId, CancellationToken ct = default)
        => _db.Tenants
              .Where(t => t.ApartmentId == apartmentId)
              .OrderByDescending(t => t.MoveInDate)
              .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public Task UpdateAsync(Tenant tenant, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        _db.Tenants.Update(tenant);
        return Task.CompletedTask;
    }
}
