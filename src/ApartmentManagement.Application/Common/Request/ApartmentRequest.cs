using ApartmentManagement.Application.Common;
using ApartmentManagement.Domain.Leasing.Apartments;

public sealed record ApartmentRequest(
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
    ApartmentStatus Status
);
