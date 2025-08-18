using ApartmentManagement.Domain.Leasing.Apartments;
using MediatR;

public record ApartmentDto(
    Guid Id,
    string Name,
    int UnitNumber,
    AddressDto Address,
    int Bedrooms,
    int Bathrooms,
    int? SquareFeet,
    decimal MonthlyRent,
    DateOnly? AvailableFrom,
    bool IsAvailable,
    string? Description,
    string[] Amenities,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
public record GetApartmentQuery(Guid Id) : IRequest<ApartmentDto?>;
public sealed class GetApartmentHandler : IRequestHandler<GetApartmentQuery, ApartmentDto?>
{
    private readonly IApartmentRepository _repo;
    public GetApartmentHandler(IApartmentRepository repo) => _repo = repo;

    public async Task<ApartmentDto?> Handle(GetApartmentQuery q, CancellationToken ct)
    {
        var a = await _repo.GetByIdAsync(new ApartmentId(q.Id), ct);
        if (a is null) return null;

        var dto = new ApartmentDto(
            Id: a.Id.Value,
            Name: a.Name,
            UnitNumber: a.UnitNumber,
            Address: new AddressDto(a.Address.Line1, a.Address.City, a.Address.State, a.Address.PostalCode),
            Bedrooms: a.Bedrooms,
            Bathrooms: a.Bathrooms,
            SquareFeet: a.SquareFeet,
            MonthlyRent: a.MonthlyRent,
            AvailableFrom: a.AvailableFrom,
            IsAvailable: a.IsAvailable,
            Description: a.Description,
            Amenities: ToAmenityNames(a.Amenities),
            CreatedAt: a.CreatedAt,
            UpdatedAt: a.UpdatedAt
        );

        return dto;
    }

    private static string[] ToAmenityNames(Amenities flags) =>
        Enum.GetValues<Amenities>()
            .Where(f => f != Amenities.None && flags.HasFlag(f))
            .Select(f => f.ToString())
            .ToArray();
}
