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
    int? SquareFeet,
    DateOnly? AvailableFrom,
    string? Description,
    IReadOnlyCollection<string>? Amenities,
    ApartmentStatus Status
);
