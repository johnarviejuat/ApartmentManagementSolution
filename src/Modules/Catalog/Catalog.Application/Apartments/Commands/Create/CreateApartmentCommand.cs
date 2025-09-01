using Catalog.Application.Common;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Apartments.Commands.Create;

public record CreateApartmentCommand(
    string Name,
    int UnitNumber,
    string Line1,
    string City,
    string State,
    string PostalCode,
    int Bedrooms,
    int Bathrooms,
    int Capacity,
    decimal MonthlyRent,
    decimal? AdvanceRent,
    decimal ? SecurityDeposit,
    int? SquareFeet,
    DateOnly? AvailableFrom,
    string? Description,
    ApartmentStatus Status = 0
) : IRequest<Guid>;
