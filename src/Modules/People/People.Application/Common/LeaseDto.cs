namespace People.Application.Common
{
    public sealed record LeaseDto(
        Guid Id,
        decimal MonthlyRent,
        DateOnly NextDueDate,
        decimal Credit,
        decimal DepositRequired,
        decimal DepositHeld,
        bool IsDepositFunded,
        ApartmentDto? Apartment = null
    );
}
