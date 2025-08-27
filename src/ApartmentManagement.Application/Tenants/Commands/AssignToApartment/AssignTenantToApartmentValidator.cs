using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Leases;
using ApartmentManagement.Domain.Leasing.Payments;
using ApartmentManagement.Domain.Leasing.Tenants;
using FluentValidation;

namespace ApartmentManagement.Application.Tenants.Commands.AssignToApartment
{
    public sealed class AssignTenantToApartmentValidator : AbstractValidator<AssignTenantToApartmentCommand>
    {
        private readonly IApartmentRepository _apartmentRepo;
        private readonly IPaymentRepository _paymentRepo;

        public AssignTenantToApartmentValidator(IApartmentRepository apartmentRepo, ILeaseRepository leaseRepo)
        {
            _apartmentRepo = apartmentRepo;

            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.ApartmentId).NotEmpty();

            RuleFor(x => x.ApartmentId)
                .MustAsync(BeApartmentNotFull)
                .WithMessage("The apartment is already full.");

            RuleFor(c => c)
               .MustAsync(async (cmd, ct) =>
               {
                   var ok = await leaseRepo.ExistsForTenantApartmentAsync(
                       new TenantId(cmd.TenantId),
                       new ApartmentId(cmd.ApartmentId),
                       ct);
                   return ok;
               })
               .WithMessage(c => $"No verified payment found for Tenant '{c.TenantId}' and Apartment '{c.ApartmentId}'.");
        }

        private async Task<bool> BeApartmentNotFull(Guid apartmentId, CancellationToken ct)
        {
            var apartment = await _apartmentRepo.GetByIdAsync(new ApartmentId(apartmentId), ct);
            return apartment != null && apartment.Capacity > apartment.CurrentCapacity;
        }
    }
}
