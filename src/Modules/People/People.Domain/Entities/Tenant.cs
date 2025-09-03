

using People.Domain.ValueObjects;

namespace People.Domain.Entities;

public sealed record TenantId(Guid Value);


public enum TenantStatus
{
    Active = 0,
    Inactive = 1,
    Evicted = 2,
    Left = 3
}

public sealed class Tenant : IAggregateRoot
{
    private Tenant() { }

    public Tenant(
        TenantId id,
        PersonName name,
        Email email,
        Phone? phone = null,
        DateOnly? moveInDate = null,
        DateOnly? moveOutDate = null,
        string? notes = null,
        Guid? apartmentId = null
    )
    {
        Id = id;
        ChangeName(name);
        ChangeEmail(email);
        ChangePhone(phone);
        MoveInDate = moveInDate;
        MoveOutDate = moveOutDate;
        UpdateNotes(notes);
        Status = TenantStatus.Active;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;

        if (apartmentId is not null) AssignToApartment(apartmentId.Value);
    }

    // Identity
    public TenantId Id { get; private set; } = default!;
    public PersonName Name { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public Phone? Phone { get; private set; }

    // Leasing details
    public Guid? ApartmentId { get; private set; }
    public DateOnly? MoveInDate { get; private set; }
    public DateOnly? MoveOutDate { get; private set; }

    // Meta
    public string? Notes { get; private set; }
    public TenantStatus Status { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // --- Behavior ---
    public void ChangeName(PersonName name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Touch();
    }
    public void ChangeEmail(Email email)
    {
        if (email is null || string.IsNullOrWhiteSpace(email.Value))
            throw new ArgumentException("Email is required.", nameof(email));
        Email = email;
        Touch();
    }
    public void ChangePhone(Phone? phone)
    {
        Phone = phone;
        Touch();
    }
    public void UpdateNotes(string? notes)
    {
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        Touch();
    }
    public void AssignToApartment(Guid apartmentId)
    {
        ApartmentId = apartmentId;
        Status = TenantStatus.Active;
        MoveInDate = DateOnly.FromDateTime(DateTime.UtcNow);
        MoveOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6));
        Touch();
    }
    public void RenewAssignToApartment(DateOnly newStartDate)
    {
        Status = TenantStatus.Active;
        MoveInDate = newStartDate;
        MoveOutDate = newStartDate.AddMonths(6);
        Touch();
    }
    public void ChangeTenantStatus(TenantStatus TenantStatus) {
        Status = TenantStatus;
        ApartmentId = null;
        MoveOutDate = null;
        MoveInDate = null;
    }
    public void SoftDelete()
    {
        IsDeleted = true;
        Touch();
    }
    private void Touch() => UpdatedAt = DateTime.UtcNow;
}
