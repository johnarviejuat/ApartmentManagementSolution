using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using Leasing.Domain.Abstraction;
using MediatR;
using People.Domain.Abstraction;
using People.Domain.Entities;

namespace People.Application.Tenants.Commands.RemoveToApartment
{
    internal class RemoveTenantFromApartmentHandler(
        ITenantRepository tenantRepo,
        IApartmentRepository apartmentRepo,
        ILeaseRepository leaseRepo
    ) : IRequestHandler<RemoveTenantFromApartmentCommand, bool>
    {
        private readonly ITenantRepository _tenantRepo = tenantRepo;
        private readonly IApartmentRepository _apartmentRepo = apartmentRepo;
        private readonly ILeaseRepository _leaseRepo = leaseRepo;

        public async Task<bool> Handle(RemoveTenantFromApartmentCommand c, CancellationToken ct)
        {
            var apartment = await _apartmentRepo.GetByIdAsync(new ApartmentId(c.ApartmentId), ct)
                ?? throw new KeyNotFoundException($"Apartment '{c.ApartmentId}' not found.");

            var tenant = await _tenantRepo.GetByIdAsync(new TenantId(c.TenantId), ct)
                ?? throw new KeyNotFoundException($"Tenant '{c.TenantId}' not found.");

            var lease = await _leaseRepo.GetActiveAsync(c.TenantId, c.ApartmentId, ct);

            tenant.ChangeTenantStatus(c.TenantStatus);
            await _tenantRepo.UpdateAsync(tenant, ct);

            if (apartment.CurrentCapacity > 0)
                apartment.DecrementCurrentCapacity();

            if (apartment.Capacity > apartment.CurrentCapacity)
            {
                apartment.ChangeStatus(ApartmentStatus.Vacant);
                apartment.SetIsAvailable(true);
            }
            await _apartmentRepo.UpdateAsync(apartment, ct);

            if (lease is not null)
            {
                lease.Terminate(DateOnly.FromDateTime(DateTime.UtcNow));
                await _leaseRepo.SaveChangesAsync(ct);
            }

            await _tenantRepo.SaveChangesAsync(ct);
            await _apartmentRepo.SaveChangesAsync(ct);
            
            return true;
        }
    }
}
