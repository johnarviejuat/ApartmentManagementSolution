namespace Catalog.Application.Common
{
    public record ApartmentDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int UnitNumber { get; init; }
        public string Line1 { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string State { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;
        public int Bedrooms { get; init; }
        public int Bathrooms { get; init; }
        public int Capacity { get; init; }
        public int CurrentCapacity { get; init; }
        public int? SquareFeet { get; init; }
        public decimal MonthlyRent { get; init; }
        public decimal AdvanceRent { get; init; }
        public decimal SecurityDeposit { get; init; }
        public DateOnly? AvailableFrom { get; init; }
        public bool IsAvailable { get; init; }
        public string? Description { get; init; }
        public int Status { get; init; }
        public PersonInformation? Owner { get; init; }
        public IReadOnlyList<PersonInformation> Tenants { get; init; } = [];
    }

    public record PersonInformation
    {
        public string Id { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? Phone { get; init; }
    }
}
