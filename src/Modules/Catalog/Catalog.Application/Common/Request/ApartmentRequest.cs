using Catalog.Domain.Entities;

namespace Catalog.Application.Common.Request;
public sealed record ApartmentRequest(
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
    decimal? SecurityDeposit,
    int? SquareFeet,
    DateOnly? AvailableFrom,
    string? Description,
    ApartmentStatus Status
);
