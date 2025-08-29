using Catalog.Application.Common;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Apartments.Commands.Update;

public record UpdateApartmentCommand(
    Guid Id,
    string Name,
    int UnitNumber,
    AddressDto Address,
    int Bedrooms,
    int Bathrooms,
    int Capacity,
    decimal MonthlyRent,
    decimal? AdvanceRent,
    decimal? SecurityDeposit,
    int? SquareFeet,
    DateOnly? AvailableFrom,
    string? Description,
    ApartmentStatus Status = 0
) : IRequest<bool>;