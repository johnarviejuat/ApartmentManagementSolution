using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using Leasing.Domain.Abstraction;
using MediatR;
using People.Domain.Abstraction;
using People.Domain.Entities;

namespace People.Application.Tenants.Commands.RemoveToApartment
{
    internal class RemoveTenantFromApartmentHandler(ITenantRepository tenantRepo, IApartmentRepository apartmentRepo, ILeaseRepository leaseRepo) : IRequestHandler<RemoveTenantFromApartmentCommand, bool>
    {
        private readonly ITenantRepository _tenantRepo = tenantRepo;
        private readonly IApartmentRepository _apartmentRepo = apartmentRepo;
        private readonly ILeaseRepository _leaseRepo = leaseRepo;

        public async Task<bool> Handle(RemoveTenantFromApartmentCommand c, CancellationToken ct)
        {
            // 1) Load the apartment and tenant
            var apartment = await _apartmentRepo.GetByIdAsync(new ApartmentId(c.ApartmentId), ct);
            var tenant = await _tenantRepo.GetByIdAsync(new TenantId(c.TenantId), ct);
            var lease = await _leaseRepo.GetActiveAsync(c.TenantId, c.ApartmentId, ct);


            // 2) Evict the tenant (mark them as vacated)
            tenant?.ChangeTenantStatus(c.TenantStatus);
            await _tenantRepo.UpdateAsync(tenant!, ct);
            await _tenantRepo.SaveChangesAsync(ct);

            // 3) Decrement the current capacity of the apartment
            apartment?.DecrementCurrentCapacity();

            // 4) If the apartment is no longer full, change its status to Vacant and set availability
            if (apartment?.Capacity > apartment?.CurrentCapacity)
            {
                apartment.ChangeStatus(ApartmentStatus.Vacant);
                apartment.SetIsAvailable(true);
            }

            // 5) Remove Lease associated with the tenant and apartment
            if (lease is not null)
            {
                lease.Terminate();
                await _leaseRepo.SaveChangesAsync(ct);
            }

            // 6) Save changes to the apartment
            await _apartmentRepo.UpdateAsync(apartment!, ct);
            await _apartmentRepo.SaveChangesAsync(ct);

            return true;
        }
    }
}
