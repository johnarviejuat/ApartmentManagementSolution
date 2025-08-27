using MediatR;

namespace ApartmentManagement.Application.Tenants.Commands.RenewToApartment;
public sealed record RenewToaApartmentCommand(
    Guid TenantId,
    Guid ApartmentId,
    DateOnly NewStartDate,
    decimal NewMonthlyRent,
    decimal? NewDepositRequired,
    bool CarryOverCredit,
    bool CarryOverDeposit
) : IRequest;