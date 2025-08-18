using ApartmentManagement.Domain.Leasing.Apartments;
using FluentValidation;
using MediatR;
using System.Text.Json.Serialization;

public sealed record AddressDto(string Line1, string City, string State, string PostalCode);
public sealed record CreateApartmentCommand(
    string Name,
    int UnitNumber,
    AddressDto Address,
    int Bedrooms,
    int Bathrooms,
    decimal MonthlyRent,
    int? SquareFeet,
    DateOnly? AvailableFrom,
    string? Description,
    IReadOnlyCollection<string>? Amenities
) : IRequest<Guid>;

public sealed class CreateApartmentValidator : AbstractValidator<CreateApartmentCommand>
{
    public CreateApartmentValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.UnitNumber).GreaterThan(0);

        RuleFor(x => x.Address.Line1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Address.State).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Address.PostalCode).NotEmpty().MaximumLength(20);

        RuleFor(x => x.Bedrooms).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Bathrooms).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SquareFeet).GreaterThan(0).When(x => x.SquareFeet.HasValue);

        RuleFor(x => x.MonthlyRent).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => !string.IsNullOrWhiteSpace(x.Description));

        // Optional: validate amenities are known names
        RuleForEach(x => x.Amenities!).Must(BeKnownAmenity)
            .When(x => x.Amenities is not null)
            .WithMessage("Unknown amenity: {PropertyValue}");
    }

    private static bool BeKnownAmenity(string amenity) =>
        Enum.TryParse<Amenities>(amenity, ignoreCase: true, out _);
}

public sealed class CreateApartmentHandler : IRequestHandler<CreateApartmentCommand, Guid>
{
    private readonly IApartmentRepository _repo;
    public CreateApartmentHandler(IApartmentRepository repo) => _repo = repo;

    public async Task<Guid> Handle(CreateApartmentCommand c, CancellationToken ct)
    {
        var amenities = MergeAmenitiesFlags(c.Amenities);

        var entity = new Apartment(
            new ApartmentId(Guid.NewGuid()),
            c.Name,
            c.UnitNumber,
            new Address(c.Address.Line1, c.Address.City, c.Address.State, c.Address.PostalCode),
            c.Bedrooms,
            c.Bathrooms,
            c.MonthlyRent,
            c.SquareFeet,
            c.AvailableFrom,
            amenities
        );

        if (!string.IsNullOrWhiteSpace(c.Description))
            entity.UpdateDescription(c.Description);

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return entity.Id.Value;
    }

    private static Amenities MergeAmenitiesFlags(IReadOnlyCollection<string>? names)
    {
        if (names is null || names.Count == 0) return Amenities.None;

        Amenities flags = Amenities.None;
        foreach (var n in names)
        {
            if (Enum.TryParse<Amenities>(n, ignoreCase: true, out var parsed))
                flags |= parsed;
        }
        return flags;
    }
}
