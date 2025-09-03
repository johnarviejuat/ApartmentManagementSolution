using FluentValidation;
using MediatR;
using People.Application.Tenants.Commands.Create;
using People.Domain.Abstraction;
using People.Domain.Entities;
using People.Domain.ValueObjects;

namespace People.Application.Tenants;

public sealed class CreateTenantHandler(
    ITenantRepository tenantRepo,
    IValidator<CreateTenantCommand> validator) : IRequestHandler<CreateTenantCommand, Guid>
{
    private readonly ITenantRepository _tenantRepo = tenantRepo;
    private readonly IValidator<CreateTenantCommand> _validator = validator;

    public async Task<Guid> Handle(CreateTenantCommand c, CancellationToken ct)
    {
        var result = await _validator.ValidateAsync(c, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        Guid? apartmentId = c.ApartmentId;

        var tenant = new Tenant(
            id: new TenantId(Guid.NewGuid()),
            name: new PersonName(c.FirstName, c.LastName),
            email: new Email(c.Email),
            phone: string.IsNullOrWhiteSpace(c.Phone) ? null : new Phone(c.Phone),
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
