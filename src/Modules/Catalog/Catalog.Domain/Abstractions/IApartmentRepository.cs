using Catalog.Domain.Entities;
using People.Domain.Entities;

namespace Catalog.Domain.Abstractions;

public interface IApartmentRepository
{
    Task AddAsync(Apartment apartment, CancellationToken ct = default);
    Task<Apartment?> GetByIdAsync(ApartmentId id, CancellationToken ct = default);
    Task<List<Apartment>> GetAllAsync(CancellationToken ct = default);
    Task<bool> AnyActiveByOwnerId(OwnerId id, CancellationToken ct = default);
    Task UpdateAsync(Apartment apartment, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
