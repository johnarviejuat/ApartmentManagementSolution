namespace ApartmentManagement.Domain.Leasing.Apartments;

public sealed record ApartmentId(Guid Value);
public sealed record Address(string Line1, string City, string State, string PostalCode);
public interface IAggregateRoot { }

[Flags]
public enum Amenities
{
    None = 0,
    Parking = 1 << 0,
    Laundry = 1 << 1,
    AirConditioning = 1 << 2,
    Dishwasher = 1 << 3,
    Balcony = 1 << 4,
    Gym = 1 << 5,
    Pool = 1 << 6
}

public sealed class Apartment : IAggregateRoot
{
    private Apartment() { }

    public Apartment(
        ApartmentId id,
        string name,
        int unitNumber,
        Address address,
        int bedrooms,
        int bathrooms,
        decimal monthlyRent,
        int? squareFeet = null,
        DateOnly? availableFrom = null,
        Amenities amenities = Amenities.None,
        string? description = null)
    {
        Id = id;
        Rename(name);
        SetUnitNumber(unitNumber);
        ChangeAddress(address);
        SetBedrooms(bedrooms);
        SetBathrooms(bathrooms);
        ChangeMonthlyRent(monthlyRent);
        SquareFeet = squareFeet;
        AvailableFrom = availableFrom;
        Amenities = amenities;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        IsAvailable = availableFrom is null || availableFrom <= DateOnly.FromDateTime(DateTime.UtcNow);
    }

    // Identity
    public ApartmentId Id { get; private set; } = default!;


    // Core info
    public string Name { get; private set; } = default!;
    public int UnitNumber { get; private set; }
    public Address Address { get; private set; } = default!;
    public int Bedrooms { get; private set; }
    public int Bathrooms { get; private set; }
    public int? SquareFeet { get; private set; }


    // Pricing & availability (simple decimal for rent; introduce Money VO later if needed)
    public decimal MonthlyRent { get; private set; }
    public DateOnly? AvailableFrom { get; private set; }
    public bool IsAvailable { get; private set; }

    // Extras
    public Amenities Amenities { get; private set; }
    public string? Description { get; private set; }

    // Soft-delete & audit
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }


    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        Name = name.Trim(); Touch();
    }

    public void SetUnitNumber(int unitNumber)
    {
        if (unitNumber <= 0) throw new ArgumentOutOfRangeException(nameof(unitNumber));
        UnitNumber = unitNumber; Touch();
    }

    public void ChangeAddress(Address address)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address)); Touch();
    }

    public void SetBedrooms(int bedrooms)
    {
        if (bedrooms < 0) throw new ArgumentOutOfRangeException(nameof(bedrooms));
        Bedrooms = bedrooms; Touch();
    }

    public void SetBathrooms(int bathrooms)
    {
        if (bathrooms < 0) throw new ArgumentOutOfRangeException(nameof(bathrooms));
        Bathrooms = bathrooms; Touch();
    }

    public void ChangeMonthlyRent(decimal monthlyRent)
    {
        if (monthlyRent < 0) throw new ArgumentOutOfRangeException(nameof(monthlyRent));
        MonthlyRent = monthlyRent; Touch();
    }

    public void SetAvailableFrom(DateOnly? date)
    {
        AvailableFrom = date;
        IsAvailable = date is null || date <= DateOnly.FromDateTime(DateTime.UtcNow);
        Touch();
    }

    public void SetAmenities(Amenities amenities) { Amenities = amenities; Touch(); }

    public void UpdateDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(); Touch();
    }

    public void MarkUnavailable() { IsAvailable = false; Touch(); }

    public void SoftDelete() { IsDeleted = true; Touch(); }

    private void Touch() => UpdatedAt = DateTime.UtcNow;
}
