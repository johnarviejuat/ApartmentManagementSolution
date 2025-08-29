
using Leasing.Domain.Abstraction;
using MediatR;

namespace People.Application.Tenants.Commands.RenewToApartment
{
    internal class RenewToApartmentHandler(ILeaseRepository leaseRepo) : IRequestHandler<RenewToaApartmentCommand>
    {
        private readonly ILeaseRepository _leaseRepo = leaseRepo;

        public async Task Handle(RenewToaApartmentCommand req, CancellationToken ct)
        {
            var tenantId = req.TenantId;
            var apartmentId = req.ApartmentId;

            var current = await _leaseRepo.GetActiveAsync(tenantId, apartmentId, ct) ?? throw new KeyNotFoundException($"No active lease for tenant '{req.TenantId}' and apartment '{req.ApartmentId}'.");

            var newFirstDueDate = ComputeFirstDueDate(req.NewStartDate);

            var next = current.Renew(
                newStartDate: req.NewStartDate,
                newMonthlyRent: req.NewMonthlyRent,
                newFirstDueDate: newFirstDueDate,
                newDepositRequired: req.NewDepositRequired,
                carryOverCredit: req.CarryOverCredit,
                carryOverDeposit: req.CarryOverDeposit);

            await _leaseRepo.AddAsync(next, ct);
            await _leaseRepo.SaveChangesAsync(ct);
        }
        private static DateOnly ComputeFirstDueDate(DateOnly startDate) => startDate;
    }
}
