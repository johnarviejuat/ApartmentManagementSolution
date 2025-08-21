using ApartmentManagement.Domain.Leasing.Owners;
using ApartmentManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Infrastructure.Repositories;
public sealed class OwnerRepository(AppDbContext db) : IOwnerRepository
{
    private readonly AppDbContext _db = db;

    public async Task AddAsync(Owner owner, CancellationToken ct = default)
        => await _db.Owners.AddAsync(owner, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public async Task<Owner?> GetByIdAsync(OwnerId id, CancellationToken ct = default)
           => await _db.Owners.FirstOrDefaultAsync(o => o.Id == id, ct);
}