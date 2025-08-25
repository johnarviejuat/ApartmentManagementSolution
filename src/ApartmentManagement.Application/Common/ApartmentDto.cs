namespace ApartmentManagement.Application.Common
{
    public record ApartmentDto(
        Guid Id,
        string Name,
        int UnitNumber,
        AddressDto Address,
        int Bedrooms,
        int Bathrooms,
        int Capacity,
        int CurrentCapacity,
        int? SquareFeet,
        decimal MonthlyRent,
        decimal AdvanceRent,
        decimal SecurityDeposit,
        DateOnly? AvailableFrom,
        bool IsAvailable,
        string? Description,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        int Status,
        OwnerDto? Owner = null,
        IReadOnlyList<TenantDto> Tenants = null!
    );
}