using ApartmentManagement.Domain.Leasing.Apartments;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Infrastructure.Repositories;

public sealed class ApartmentRepository : IApartmentRepository
{
    private readonly AppDbContext _db;
    public ApartmentRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(Apartment apartment, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(apartment);
        await _db.Apartments.AddAsync(apartment, ct);
    }

    // Tracking (so caller can update)
    public Task<Apartment?> GetByIdAsync(ApartmentId id, CancellationToken ct = default) =>
        _db.Apartments
           .FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);

    // Optional read-only variant
    public Task<Apartment?> GetByIdNoTrackingAsync(ApartmentId id, CancellationToken ct = default) =>
        _db.Apartments
           .AsNoTracking()
           .FirstOrDefaultAsync(x => x.Id == id, ct);

    // Optional: bypass global filters (e.g., soft-delete)
    public Task<Apartment?> GetByIdIncludingDeletedAsync(ApartmentId id, CancellationToken ct = default) =>
        _db.Apartments
           .IgnoreQueryFilters()
           .FirstOrDefaultAsync(x => x.Id == id, ct);
}

