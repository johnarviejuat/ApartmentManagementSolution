namespace ApartmentManagement.Application.Common
{
    public sealed record TenantDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string? Phone,
        Guid? ApartmentId,
        DateOnly? MoveInDate,
        DateOnly? MoveOutDate,
        bool IsActive,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        ApartmentDto? Apartment = null
    );
}
