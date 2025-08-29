namespace People.Application.Common.Request
{
    public sealed record RenewLeaseRequest(
         Guid TenantId,
         Guid ApartmentId,
         DateOnly NewStartDate,
         decimal NewMonthlyRent,
         decimal? NewDepositRequired = null,
         bool CarryOverCredit = true,
         bool CarryOverDeposit = true
    );
}
