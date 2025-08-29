using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Catalog.Application.Apartments.Commands.AssignOwner;

public sealed class AssignOwnerHandler(
    IApartmentRepository apartmentRepo,
    IValidator<AssignOwnerToApartmentCommand> validator
) : IRequestHandler<AssignOwnerToApartmentCommand, Unit>
{
    public async Task<Unit> Handle(AssignOwnerToApartmentCommand c, CancellationToken ct)
    {
        var vr = await validator.ValidateAsync(c, ct);
        if (!vr.IsValid) throw new ValidationException(vr.Errors);

        var apartment = await apartmentRepo.GetByIdAsync(new ApartmentId(c.ApartmentId), ct) ?? throw new KeyNotFoundException($"Apartment '{c.ApartmentId}' not found.");

        apartment.AssignOwner(c.OwnerId);
        await apartmentRepo.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
