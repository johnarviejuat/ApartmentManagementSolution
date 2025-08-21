using ApartmentManagement.Domain.Leasing.Apartments;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Infrastructure.Repositories;

public sealed class ApartmentRepository(AppDbContext db) : IApartmentRepository
{
    private readonly AppDbContext _db = db;

    public Task AddAsync(Apartment apartment, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(apartment);
        return _db.Apartments.AddAsync(apartment, ct).AsTask();
    }

    public Task<Apartment?> GetByIdAsync(ApartmentId id, CancellationToken ct = default) =>
        _db.Apartments
           .FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<List<Apartment>> GetAllAsync(CancellationToken ct = default) =>
        _db.Apartments
           .AsNoTracking()
           .ToListAsync(ct);

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
