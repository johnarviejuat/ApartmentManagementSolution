namespace People.Application.Common
{
    public sealed record TenantDto
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? Phone { get; init; }
        public DateOnly? MoveInDate { get; init; }
        public DateOnly? MoveOutDate { get; init; }
        public bool IsActive { get; init; }
        public string? Notes { get; init; }
        public TenantApartment? Apartment { get; init; }
    }

    public sealed record TenantApartment
    {
        public required string Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string UnitNumber { get; init; } = string.Empty;
    }
}
