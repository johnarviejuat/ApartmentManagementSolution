using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Tenants;
using FluentValidation;
using MediatR;

namespace ApartmentManagement.Application.Tenants.Commands.AssignToApartment;

public sealed class AssignTenantToApartmentHandler(
    ITenantRepository tenantRepo,
    IApartmentRepository apartmentRepo,
    IValidator<AssignTenantToApartmentCommand> validator
) : IRequestHandler<AssignTenantToApartmentCommand, Unit>
{
    private readonly ITenantRepository _tenantRepo = tenantRepo;
    private readonly IApartmentRepository _apartmentRepo = apartmentRepo;
    private readonly IValidator<AssignTenantToApartmentCommand> _validator = validator;

    public async Task<Unit> Handle(AssignTenantToApartmentCommand c, CancellationToken ct)
    {
        var vr = await _validator.ValidateAsync(c, ct);
        if (!vr.IsValid) throw new ValidationException(vr.Errors);

        // 1) Load tenant (tracked)
        var tenant = await _tenantRepo.GetByIdAsync(new TenantId(c.TenantId), ct)
                     ?? throw new KeyNotFoundException($"Tenant '{c.TenantId}' not found.");

        // 2) Ensure apartment exists (avoid FK violation)
        var apartment = await _apartmentRepo.GetByIdAsync(new ApartmentId(c.ApartmentId), ct)
                        ?? throw new KeyNotFoundException($"Apartment '{c.ApartmentId}' not found.");

        // 3) Check if apartment is full (Capacity == CurrentCapacity)
        if (apartment.Capacity == apartment.CurrentCapacity)
        {
            throw new InvalidOperationException($"Apartment '{c.ApartmentId}' is already full.");
        }

        // 4) Assign tenant to apartment
        tenant.AssignToApartment(apartment.Id);

        // 5) Increment the CurrentCapacity by 1 since the tenant has been successfully assigned
        apartment.IncrementCurrentCapacity();
        if (apartment.Capacity == apartment.CurrentCapacity)
        {
            apartment.ChangeStatus(ApartmentStatus.Occupied);
            apartment.SetIsAvailable(false);
        }

        // 6) Save the changes
        await _tenantRepo.SaveChangesAsync(ct);
        await _apartmentRepo.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
