using Microsoft.EntityFrameworkCore;
using People.Domain.Abstraction;
using People.Domain.Entities;
using People.Infrastructure.Persistence;

namespace People.Infrastructure.Persistence.Repositories;

public sealed class TenantRepository(PeopleDbContext db) : ITenantRepository
{
    private readonly PeopleDbContext _db = db;

    public Task AddAsync(Tenant tenant, CancellationToken ct = default)
        => _db.Tenants.AddAsync(tenant, ct).AsTask();

    public Task<Tenant?> GetByIdAsync(TenantId id, CancellationToken ct = default)
        => _db.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct)!;

    public Task<List<Tenant>> GetAllAsync(CancellationToken ct = default) =>
    _db.Tenants
       .AsNoTracking()
       .Where(a => !a.IsDeleted)
       .ToListAsync(ct);

    public Task<bool> AnyActiveByTenantId(TenantId id, CancellationToken ct = default) =>
        _db.Tenants
        .AsNoTracking()
        .AnyAsync(a => a.Id == id && !a.IsDeleted && a.ApartmentId != null, ct);

    public Task<List<Tenant>> ListByApartmentIdAsync(Guid apartmentId, CancellationToken ct = default)
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
