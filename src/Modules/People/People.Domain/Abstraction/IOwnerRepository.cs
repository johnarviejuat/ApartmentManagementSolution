using People.Domain.Entities;

namespace People.Domain.Abstraction
{
    public interface IOwnerRepository
    {
        Task AddAsync(Owner owner, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
        Task<Owner?> GetByIdAsync(OwnerId id, CancellationToken ct = default);
        Task<List<Owner>> GetAllAsync(CancellationToken ct = default);
        Task UpdateAsync(Owner owner, CancellationToken ct = default);
    }
}
