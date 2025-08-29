using Catalog.Application.Apartments.Commands.Delete;
using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Apartments;

public sealed class DeleteApartmentHandler(IApartmentRepository repo)
    : IRequestHandler<DeleteApartmentCommand, bool>
{
    private readonly IApartmentRepository _repo = repo;

    public async Task<bool> Handle(DeleteApartmentCommand c, CancellationToken ct)
    {
        var apartment = await _repo.GetByIdAsync(new ApartmentId(c.Id), ct);

        if (apartment is null)
            return false;

        if (apartment.IsDeleted)
            return false;

        apartment.SoftDelete();

        await _repo.UpdateAsync(apartment, ct);
        await _repo.SaveChangesAsync(ct);

        return true;
    }
}
