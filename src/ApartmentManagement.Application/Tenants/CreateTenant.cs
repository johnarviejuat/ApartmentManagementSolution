using ApartmentManagement.Application.Tenants.Commands.Create;
using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Tenants;
using FluentValidation;
using MediatR;

namespace ApartmentManagement.Application.Tenants;

public sealed class CreateTenantHandler(
    ITenantRepository tenantRepo,
    IApartmentRepository apartmentRepo,
    IValidator<CreateTenantCommand> validator
) : IRequestHandler<CreateTenantCommand, Guid>
{
    private readonly ITenantRepository _tenantRepo = tenantRepo;
    private readonly IApartmentRepository _apartmentRepo = apartmentRepo;
    private readonly IValidator<CreateTenantCommand> _validator = validator;

    public async Task<Guid> Handle(CreateTenantCommand c, CancellationToken ct)
    {
        var result = await _validator.ValidateAsync(c, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        ApartmentId? apartmentId = null;
        if (c.ApartmentId is Guid aptId)
        {
            _ = await _apartmentRepo.GetByIdAsync(new ApartmentId(aptId), ct) ?? throw new KeyNotFoundException($"Apartment '{aptId}' was not found.");
            apartmentId = new ApartmentId(aptId);
        }

        var tenant = new Tenant(
            id: new TenantId(Guid.NewGuid()),
            name: new TenantName(c.FirstName, c.LastName),
            email: new TenantEmail(c.Email),
            phone: string.IsNullOrWhiteSpace(c.Phone) ? null : new TenantPhone(c.Phone),
            moveInDate: c.MoveInDate,
            moveOutDate: c.MoveOutDate,
            notes: string.IsNullOrWhiteSpace(c.Notes) ? null : c.Notes,
            apartmentId: apartmentId
        );

        await _tenantRepo.AddAsync(tenant, ct);
        await _tenantRepo.SaveChangesAsync(ct);

        return tenant.Id.Value;
    }
}
