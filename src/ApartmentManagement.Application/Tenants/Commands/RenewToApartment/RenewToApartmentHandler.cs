using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Leases;
using ApartmentManagement.Domain.Leasing.Tenants;
using MediatR;


namespace ApartmentManagement.Application.Tenants.Commands.RenewToApartment
{
    internal class RenewToApartmentHandler(ILeaseRepository leaseRepo) : IRequestHandler<RenewToaApartmentCommand>
    {
        private readonly ILeaseRepository _leaseRepo = leaseRepo;

        public async Task Handle(RenewToaApartmentCommand req, CancellationToken ct)
        {
            var tenantId = new TenantId(req.TenantId);
            var apartmentId = new ApartmentId(req.ApartmentId);

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
