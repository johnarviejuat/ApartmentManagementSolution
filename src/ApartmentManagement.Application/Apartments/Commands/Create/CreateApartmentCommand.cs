using MediatR;

namespace ApartmentManagement.Application.Apartments.Commands.Create;

public record AddressDto(string Line1, string City, string State, string PostalCode);

public record CreateApartmentCommand(
    string Name,
    int UnitNumber,
    AddressDto Address
) : IRequest<Guid>;
