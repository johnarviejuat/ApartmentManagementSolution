namespace ApartmentManagement.Domain.Leasing.Payments
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
        Task<bool> ExistsByReferenceAsync(string referenceNumber, CancellationToken ct = default);
        Task<Payment?> GetByReferenceAsync(string referenceNumber, CancellationToken ct = default);
        Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}
