using ApartmentManagement.Domain.Leasing.Apartments;
using FluentValidation;

namespace ApartmentManagement.Application.Tenants.Commands.AssignToApartment
{
    public sealed class AssignTenantToApartmentValidator : AbstractValidator<AssignTenantToApartmentCommand>
    {
        private readonly IApartmentRepository _apartmentRepo;

        public AssignTenantToApartmentValidator(IApartmentRepository apartmentRepo)
        {
            _apartmentRepo = apartmentRepo;

            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.ApartmentId).NotEmpty();

            // Custom validation to check if the apartment is full
            RuleFor(x => x.ApartmentId)
                .MustAsync(BeApartmentNotFull)
                .WithMessage("The apartment is already full.");
        }

        private async Task<bool> BeApartmentNotFull(Guid apartmentId, CancellationToken ct)
        {
            var apartment = await _apartmentRepo.GetByIdAsync(new ApartmentId(apartmentId), ct);
            return apartment != null && apartment.Capacity > apartment.CurrentCapacity;
        }
    }
}
