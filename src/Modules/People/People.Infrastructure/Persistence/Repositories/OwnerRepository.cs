using Microsoft.EntityFrameworkCore;
using People.Domain.Abstraction;
using People.Domain.Entities;
using People.Infrastructure.Persistence;

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
              .FirstOrDefaultAsync(o => o.Id == id, ct);
}