using ApartmentManagement.Domain.Leasing.Apartments;
namespace ApartmentManagement.Domain.Leasing.Owners;

public sealed record OwnerId(Guid Value);
public sealed record PersonName(string First, string Last)
{
    public override string ToString() => $"{First} {Last}".Trim();
}
public sealed record Email(string Value)
{
    public override string ToString() => Value;
}
public sealed record Phone(string Value)
{
    public override string ToString() => Value;
}
public sealed class Owner : IAggregateRoot
{
    private Owner() { }

    public Owner(OwnerId id, PersonName name, Email email, Phone? phone = null, Address? mailingAddress = null, string? notes = null)
    {
        Id = id;
        ChangeName(name);
        ChangeEmail(email);
        ChangePhone(phone);
        ChangeMailingAddress(mailingAddress);
        UpdateNotes(notes);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        IsActive = true;
    }

    public OwnerId Id { get; private set; } = default!;
    public PersonName Name { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public Phone? Phone { get; private set; }
    public Address? MailingAddress { get; private set; }
    public string? Notes { get; private set; }

    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

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
    public void ChangePhone(Phone? phone) { Phone = phone; Touch(); }
    public void ChangeMailingAddress(Address? address) { MailingAddress = address; Touch(); }
    public void UpdateNotes(string? notes) { Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(); Touch(); }
    public void Deactivate() { IsActive = false; Touch(); }
    public void Activate() { IsActive = true; Touch(); }
    public void SoftDelete() { IsDeleted = true; Touch(); }
    private void Touch() => UpdatedAt = DateTime.UtcNow;
    public void AssignToApartment(ApartmentId apartmentId)
    {
        throw new NotImplementedException();
    }
}
