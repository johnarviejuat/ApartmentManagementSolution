using ApartmentManagement.Domain.Leasing.Apartments;

namespace ApartmentManagement.Domain.Leasing.Tenants;

public sealed record TenantId(Guid Value);

public sealed record TenantName(string First, string Last)
{
    public override string ToString() => $"{First} {Last}".Trim();
}

public sealed record TenantEmail(string Value)
{
    public override string ToString() => Value;
}

public sealed record TenantPhone(string Value)
{
    public override string ToString() => Value;
}

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
        TenantName name,
        TenantEmail email,
        TenantPhone? phone = null,
        DateOnly? moveInDate = null,
        DateOnly? moveOutDate = null,
        string? notes = null,
        ApartmentId? apartmentId = null
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

        if (apartmentId is not null) AssignToApartment(apartmentId);
    }

    // Identity
    public TenantId Id { get; private set; } = default!;
    public TenantName Name { get; private set; } = default!;
    public TenantEmail Email { get; private set; } = default!;
    public TenantPhone? Phone { get; private set; }

    // Leasing details
    public ApartmentId? ApartmentId { get; private set; }
    public DateOnly? MoveInDate { get; private set; }
    public DateOnly? MoveOutDate { get; private set; }

    // Meta
    public string? Notes { get; private set; }
    public TenantStatus Status { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // --- Behavior ---
    public void ChangeName(TenantName name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Touch();
    }

    public void ChangeEmail(TenantEmail email)
    {
        if (email is null || string.IsNullOrWhiteSpace(email.Value))
            throw new ArgumentException("Email is required.", nameof(email));
        Email = email;
        Touch();
    }

    public void ChangePhone(TenantPhone? phone)
    {
        Phone = phone;
        Touch();
    }

    public void UpdateNotes(string? notes)
    {
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        Touch();
    }

    public void AssignToApartment(ApartmentId apartmentId)
    {
        ApartmentId = apartmentId ?? throw new ArgumentNullException(nameof(apartmentId));
        Status = TenantStatus.Active;
        MoveInDate = DateOnly.FromDateTime(DateTime.UtcNow);
        MoveOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6));
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
