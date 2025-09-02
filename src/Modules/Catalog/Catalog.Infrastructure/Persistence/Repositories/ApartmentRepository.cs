using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using People.Domain.Entities;

namespace Catalog.Infrastructure.Persistence.Repositories;

public sealed class ApartmentRepository(CatalogDbContext db) : IApartmentRepository
{
    private readonly CatalogDbContext _db = db;

    public Task AddAsync(Apartment apartment, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(apartment);
        return _db.Apartments.AddAsync(apartment, ct).AsTask();
    }

    public Task<Apartment?> GetByIdAsync(ApartmentId id, CancellationToken ct = default) =>
       _db.Apartments.FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted == false, ct);

    public Task<List<Apartment>> GetAllAsync(CancellationToken ct = default) =>
        _db.Apartments
           .AsNoTracking()
           .Where(a => !a.IsDeleted)
           .ToListAsync(ct);

    public Task<bool> AnyActiveByOwnerId(OwnerId id, CancellationToken ct = default) =>
        _db.Apartments
           .AsNoTracking()
           .AnyAsync(a => !a.IsDeleted && a.OwnerId == id.Value, ct);

    public Task UpdateAsync(Apartment apartment, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(apartment);
        _db.Apartments.Update(apartment);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);

    public async Task<bool> IsApartmentFullAsync(ApartmentId id, CancellationToken ct = default)
    {
        var apartment = await _db.Apartments.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (apartment == null)
        {
            return false;
        }
        return apartment.Capacity == apartment.CurrentCapacity;
    }
}
