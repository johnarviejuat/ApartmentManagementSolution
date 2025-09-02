using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using People.Domain.Abstraction;
using People.Domain.Entities;

namespace People.Infrastructure.Persistence.Repositories;
public sealed class OwnerRepository(PeopleDbContext db) : IOwnerRepository
{
    private readonly PeopleDbContext _db = db;

    public async Task AddAsync(Owner owner, CancellationToken ct = default)
        => await _db.Owners.AddAsync(owner, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public Task<Owner?> GetByIdAsync(OwnerId id, CancellationToken ct = default) =>
           _db.Owners
              .AsNoTracking()
              .FirstOrDefaultAsync(o => o.Id == id && o.IsDeleted == false, ct);

    public Task<List<Owner>> GetAllAsync(CancellationToken ct = default) =>
        _db.Owners
           .AsNoTracking()
           .Where(a => !a.IsDeleted)
           .ToListAsync(ct);

    public Task UpdateAsync(Owner owner, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(owner);
        _db.Owners.Update(owner);
        return Task.CompletedTask;
    }
}