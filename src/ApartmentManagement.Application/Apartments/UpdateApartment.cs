using ApartmentManagement.Application.Apartments.Commands.Update;
using ApartmentManagement.Domain.Leasing.Apartments;
using FluentValidation;
using MediatR;

public sealed class UpdateApartmentHandler(IApartmentRepository repo, IValidator<UpdateApartmentCommand> validator) : IRequestHandler<UpdateApartmentCommand, bool>
{
    private readonly IApartmentRepository _repo = repo;
    private readonly IValidator<UpdateApartmentCommand> _validator = validator;

    public async Task<bool> Handle(UpdateApartmentCommand c, CancellationToken ct)
    {
        var result = await _validator.ValidateAsync(c, ct);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var existingApartment = await _repo.GetByIdAsync(new ApartmentId(c.Id), ct);

        if (existingApartment is null) return false;

        existingApartment.Rename(c.Name);
        existingApartment.SetUnitNumber(c.UnitNumber);
        existingApartment.ChangeAddress(new Address(c.Address.Line1, c.Address.City, c.Address.State, c.Address.PostalCode));
        existingApartment.SetBedrooms(c.Bedrooms);
        existingApartment.SetBathrooms(c.Bathrooms);
        existingApartment.SetCapacity(c.Capacity);
        existingApartment.ChangeMonthlyRent(c.MonthlyRent);
        existingApartment.SetSquareFeet(c.SquareFeet);
        existingApartment.SetAvailableFrom(c.AvailableFrom);
        existingApartment.UpdateDescription(c.Description);
        existingApartment.ChangeStatus(c.Status);

        await _repo.UpdateAsync(existingApartment, ct);

        await _repo.SaveChangesAsync(ct);

        return true;
    }
}
