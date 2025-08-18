namespace ApartmentManagement.Domain.Leasing.Apartments;

public interface IApartmentRepository
{
    Task AddAsync(Apartment apartment, CancellationToken ct = default);
    Task<Apartment?> GetByIdAsync(ApartmentId id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
