using Billing.Domain.Abstraction;
using Billing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Billing.Infrastructure.Persistence.Repositories
{
    public sealed class PaymentRepository(BillingDbContext db) : IPaymentRepository
    {
        private readonly BillingDbContext _db = db;
        
        public Task AddAsync(Payment payment, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(payment);
            return _db.Payments.AddAsync(payment, ct).AsTask();
        }
        public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
        public Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default) => _db.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
        public Task<bool> ExistsByReferenceAsync(string referenceNumber, CancellationToken ct = default) => _db.Payments.AsNoTracking().AnyAsync(p => p.ReferenceNumber == referenceNumber, ct);
        public Task<Payment?> GetByReferenceAsync(string referenceNumber, CancellationToken ct = default) => _db.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.ReferenceNumber == referenceNumber, ct);
    }
}
