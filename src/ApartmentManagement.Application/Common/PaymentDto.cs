namespace ApartmentManagement.Application.Common
{
    public sealed record PaymentDto(
        Guid Id,
        decimal Amount,
        DateOnly PaidAt,
        string Method,
        string ReferenceNumber,
        string Notes,
        decimal DepositPortion,
        string? Apartment = "",
        string? Tenant = ""
    );
}
