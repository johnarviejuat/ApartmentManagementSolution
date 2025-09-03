namespace Leasing.Application.Common
{
    public record LeaseDto
    {
        public Guid Id { get; init; }
        public decimal MonthlyRent { get; init; }
        public DateOnly NextDueDate { get; init; }
        public decimal Credit { get; init; }
        public decimal DepositHeld { get; init; }
        public bool IsDepositFunded { get; init; }
        public bool IsActive { get; init; }
        public ApartmentInformation Apartment { get; init; }
        public PersonInformation? Owner { get; init; }
        public PersonInformation? Tenant { get; init; }
    }

    public record ApartmentInformation
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; }
        public int UnitNumber { get; init; }
        public string Line1 { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string State { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;
        public int Capacity { get; init; }
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
