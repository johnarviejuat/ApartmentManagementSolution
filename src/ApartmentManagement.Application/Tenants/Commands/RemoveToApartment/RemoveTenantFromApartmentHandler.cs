using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Tenants;
using MediatR;

namespace ApartmentManagement.Application.Tenants.Commands.RemoveToApartment
{
    internal class RemoveTenantFromApartmentHandler(ITenantRepository tenantRepo, IApartmentRepository apartmentRepo) : IRequestHandler<RemoveTenantFromApartmentCommand, bool>
    {
        private readonly ITenantRepository _tenantRepo = tenantRepo;
        private readonly IApartmentRepository _apartmentRepo = apartmentRepo;

        public async Task<bool> Handle(RemoveTenantFromApartmentCommand c, CancellationToken ct)
        {
            // 1) Load the apartment and tenant
            var apartment = await _apartmentRepo.GetByIdAsync(new ApartmentId(c.ApartmentId), ct);
            var tenant = await _tenantRepo.GetByIdAsync(new TenantId(c.TenantId), ct);

            // 2) Evict the tenant (mark them as vacated)
            tenant.ChangeTenantStatus(c.TenantStatus);
            await _tenantRepo.UpdateAsync(tenant, ct);
            await _tenantRepo.SaveChangesAsync(ct);

            // 3) Decrement the current capacity of the apartment
            apartment.DecrementCurrentCapacity();

            // 4) If the apartment is no longer full, change its status to Vacant and set availability
            if (apartment.Capacity > apartment.CurrentCapacity)
            {
                apartment.ChangeStatus(ApartmentStatus.Vacant);  // Assuming this sets it to 'Vacant'
                apartment.SetIsAvailable(true);  // Assuming this marks it as available for future tenants
            }

            // 5) Save changes to the apartment
            await _apartmentRepo.UpdateAsync(apartment, ct);
            await _apartmentRepo.SaveChangesAsync(ct);

            return true;
        }
    }
}
