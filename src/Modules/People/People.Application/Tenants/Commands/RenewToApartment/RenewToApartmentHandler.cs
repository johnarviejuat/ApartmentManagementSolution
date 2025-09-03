using Leasing.Domain.Abstraction;
using MediatR;
using People.Domain.Abstraction;
using People.Domain.Entities;

namespace People.Application.Tenants.Commands.RenewToApartment
{
    internal class RenewToApartmentHandler(ILeaseRepository leaseRepo, ITenantRepository tenantRepo) : IRequestHandler<RenewToaApartmentCommand>
    {
        private readonly ILeaseRepository _leaseRepo = leaseRepo;
        private readonly ITenantRepository _tenantRepo = tenantRepo;  

        public async Task Handle(RenewToaApartmentCommand req, CancellationToken ct)
        {
            var tenantId = req.TenantId;
            var apartmentId = req.ApartmentId;

            var current = await _leaseRepo.GetActiveAsync(tenantId, apartmentId, ct)
                ?? throw new KeyNotFoundException($"No active lease for tenant '{req.TenantId}' and apartment '{req.ApartmentId}'.");

            var tenant = await _tenantRepo.GetByIdAsync(new TenantId(tenantId), ct)
             ?? throw new KeyNotFoundException($"Tenant '{tenantId}' not found.");
            tenant.RenewAssignToApartment(req.NewStartDate);
            await _tenantRepo.UpdateAsync(tenant, ct);
            await _tenantRepo.SaveChangesAsync(ct);

            var newFirstDueDate = ComputeNextDueDate(req.NewStartDate);

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

        private static DateOnly ComputeNextDueDate(DateOnly startDate)
        {
            var targetDay = startDate.Day;
            var nextMonth = startDate.AddMonths(1);
            var dayInNextMonth = Math.Min(targetDay, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
            return new DateOnly(nextMonth.Year, nextMonth.Month, dayInNextMonth);
        }
    }
}
