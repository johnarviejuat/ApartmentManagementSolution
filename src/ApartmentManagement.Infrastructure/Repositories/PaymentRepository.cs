using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Payments;
using ApartmentManagement.Domain.Leasing.Tenants;
using Microsoft.EntityFrameworkCore;
namespace ApartmentManagement.Infrastructure.Repositories
{
    public sealed class PaymentRepository(AppDbContext db) : IPaymentRepository
    {
        private readonly AppDbContext _db = db;
        
        public Task AddAsync(Payment payment, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(payment);
            return _db.Payments.AddAsync(payment, ct).AsTask();
        }

        public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

        public Task<bool> ExistsByReferenceAsync(string referenceNumber, CancellationToken ct = default)
            => _db.Payments.AsNoTracking().AnyAsync(p => p.ReferenceNumber == referenceNumber, ct);

        public Task<Payment?> GetByReferenceAsync(string referenceNumber, CancellationToken ct = default)
            => _db.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.ReferenceNumber == referenceNumber, ct);

        public Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _db.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

        public Task<bool> ExistsForTenantApartmentAsync(TenantId tenantId, ApartmentId apartmentId, CancellationToken ct = default) =>
            _db.Payments.AsNoTracking().AnyAsync(p =>
                    p.TenantId == tenantId &&
                    p.ApartmentId == apartmentId
                , ct);
    }
}
