using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Owners;
using FluentValidation;
using MediatR;

namespace ApartmentManagement.Application.Apartments.Commands.AssignOwner;

public sealed class AssignOwnerHandler(
    IOwnerRepository ownerRepo,
    IApartmentRepository apartmentRepo,
    IValidator<AssignOwnerToApartmentCommand> validator
) : IRequestHandler<AssignOwnerToApartmentCommand, Unit>
{
    public async Task<Unit> Handle(AssignOwnerToApartmentCommand c, CancellationToken ct)
    {
        var vr = await validator.ValidateAsync(c, ct);
        if (!vr.IsValid) throw new ValidationException(vr.Errors);

        _ = await ownerRepo.GetByIdAsync(new OwnerId(c.OwnerId), ct) ?? throw new KeyNotFoundException($"Owner '{c.OwnerId}' not found.");

        var apartment = await apartmentRepo.GetByIdAsync(new ApartmentId(c.ApartmentId), ct) ?? throw new KeyNotFoundException($"Apartment '{c.ApartmentId}' not found.");

        apartment.AssignOwner(new OwnerId(c.OwnerId));
        await apartmentRepo.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
