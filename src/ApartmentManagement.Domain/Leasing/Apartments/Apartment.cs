using ApartmentManagement.Domain.Leasing.Owners;
using ApartmentManagement.Domain.Leasing.Tenants;

namespace ApartmentManagement.Domain.Leasing.Apartments;

public sealed record ApartmentId(Guid Value);
public sealed record Address(string Line1, string City, string State, string PostalCode);
public interface IAggregateRoot { }

public enum ApartmentStatus
{
    Vacant = 0,
    Occupied = 1,
    Under_Maintenance = 2
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
     int capacity,
     decimal monthlyRent,
     decimal? advanceRent,
     decimal? securityDeposit,
     int? squareFeet = null,
     DateOnly? availableFrom = null,
     string? description = null,
     ApartmentStatus status = ApartmentStatus.Vacant,
     OwnerId? ownerId = null)
    {
        Id = id;
        Rename(name);
        SetUnitNumber(unitNumber);
        ChangeAddress(address);
        SetBedrooms(bedrooms);
        SetBathrooms(bathrooms);
        SetCapacity(capacity);
        ChangeMonthlyRent(monthlyRent);
        ChangeAdvanceRent(advanceRent);
        ChangeSecurityDeposit(securityDeposit);
        SquareFeet = squareFeet;
        AvailableFrom = availableFrom;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        IsAvailable = availableFrom is null || availableFrom <= DateOnly.FromDateTime(DateTime.UtcNow);
        Status = status;

        if (ownerId is not null) AssignOwner(ownerId);
    }

    // --- Ownership (add these) ---
    public OwnerId? OwnerId { get; private set; } = default!;
    public DateTime? OwnershipAssignedAt { get; private set; }
    public void AssignOwner(OwnerId ownerId)
    {
        OwnerId = ownerId ?? throw new ArgumentNullException(nameof(ownerId));
        OwnershipAssignedAt = DateTime.UtcNow;
        Touch();
    }
    public void TransferOwnership(OwnerId newOwnerId)
    {
        ArgumentNullException.ThrowIfNull(newOwnerId);
        if (OwnerId == newOwnerId) return;
        OwnerId = newOwnerId;
        OwnershipAssignedAt = DateTime.UtcNow;
        Touch();
    }

    // Identity
    public ApartmentId Id { get; private set; } = default!;


    // Core info
    public string Name { get; private set; } = default!;
    public int UnitNumber { get; private set; }
    public Address Address { get; private set; } = default!;
    public int Bedrooms { get; private set; }
    public int Bathrooms { get; private set; }
    public int Capacity { get; private set; }
    public int CurrentCapacity { get; private set; }
    public int? SquareFeet { get; private set; }
    public ApartmentStatus Status { get; private set; } = ApartmentStatus.Vacant;


    // Pricing & availability (simple decimal for rent; introduce Money VO later if needed)
    public decimal MonthlyRent { get; private set; }
    public decimal AdvanceRent { get; private set; }
    public decimal SecurityDeposit { get; private set; }
    public DateOnly? AvailableFrom { get; private set; }
    public bool IsAvailable { get; private set; }

    // Extras
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
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitNumber);
        UnitNumber = unitNumber; Touch();
    }

    public void ChangeAddress(Address address)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address)); Touch();
    }

    public void SetBedrooms(int bedrooms)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bedrooms);
        Bedrooms = bedrooms; Touch();
    }

    public void SetBathrooms(int bathrooms)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bathrooms);
        Bathrooms = bathrooms; Touch();
    }

    public void SetCapacity(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(capacity);
        Capacity = capacity; Touch();
    }

    public void SetSquareFeet(int? squareFeet)
    {
        if (squareFeet is < 0)
            throw new ArgumentOutOfRangeException(nameof(squareFeet));

        SquareFeet = squareFeet;
        Touch();
    }

    public void ChangeMonthlyRent(decimal monthlyRent)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(monthlyRent);
        MonthlyRent = monthlyRent; Touch();
    }

    public void ChangeAdvanceRent(decimal? advanceRent)
    {
        if (advanceRent is < 0)
            throw new ArgumentOutOfRangeException(nameof(advanceRent));
        AdvanceRent = advanceRent ?? 0;
        Touch();
    }

    public void ChangeSecurityDeposit(decimal? securityDeposit)
    {
        if (securityDeposit is < 0)
            throw new ArgumentOutOfRangeException(nameof(securityDeposit));
        SecurityDeposit = securityDeposit ?? 0;
        Touch();
    }

    public void SetAvailableFrom(DateOnly? date)
    {
        AvailableFrom = date;
        IsAvailable = date is null || date <= DateOnly.FromDateTime(DateTime.UtcNow);
        Touch();
    }

    public void SetIsAvailable(bool isAvailable)
    {
        IsAvailable = isAvailable;
        Touch();
    }

    public void UpdateDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(); Touch();
    }

    public void MarkUnavailable() { IsAvailable = false; Touch(); }

    public void SoftDelete() { IsDeleted = true; Touch(); }

    private void Touch() => UpdatedAt = DateTime.UtcNow;

    public void ChangeStatus(ApartmentStatus status)
    {
        Status = status;
        Touch();
    }

    public void IncrementCurrentCapacity()
    {
        if (CurrentCapacity < Capacity)
        {
            CurrentCapacity++;
        }
        else
        {
            throw new InvalidOperationException("Cannot increment, apartment is full.");
        }
    }
    public void DecrementCurrentCapacity()
    {
        if (CurrentCapacity > 0)
        {
            CurrentCapacity--;
        }
        else
        {
            throw new InvalidOperationException("Cannot decrement, apartment is empty.");
        }
    }

}
