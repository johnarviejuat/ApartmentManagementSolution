using ApartmentManagement.Application.Apartments.Commands.Create;
using ApartmentManagement.Domain.Leasing.Apartments;
using FluentValidation;
using MediatR;

namespace ApartmentManagement.Application.Apartments;
public sealed class CreateApartmentHandler(IApartmentRepository repo, IValidator<CreateApartmentCommand> validator) : IRequestHandler<CreateApartmentCommand, Guid>
{
    private readonly IApartmentRepository _repo = repo;
    private readonly IValidator<CreateApartmentCommand> _validator = validator;

    public async Task<Guid> Handle(CreateApartmentCommand c, CancellationToken ct)
    {
        var result = await _validator.ValidateAsync(c, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var entity = new Apartment(
            new ApartmentId(Guid.NewGuid()),
            c.Name,
            c.UnitNumber,
            new Address(c.Address.Line1, c.Address.City, c.Address.State, c.Address.PostalCode),
            c.Bedrooms,
            c.Bathrooms,
            c.Capacity,
            c.MonthlyRent,
            c.SquareFeet,
            c.AvailableFrom,
            c.Description,     
            c.Status           
        );

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        return entity.Id.Value;
    }
}
