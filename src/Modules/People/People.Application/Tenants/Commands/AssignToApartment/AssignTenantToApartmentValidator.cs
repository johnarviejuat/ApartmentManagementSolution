using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using FluentValidation;
using Leasing.Domain.Abstraction;
using People.Domain.Entities;

namespace People.Application.Tenants.Commands.AssignToApartment
{
    public sealed class AssignTenantToApartmentValidator : AbstractValidator<AssignTenantToApartmentCommand>
    {
        private readonly IApartmentRepository _apartmentRepo;

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
                       cmd.TenantId,
                       cmd.ApartmentId,
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
