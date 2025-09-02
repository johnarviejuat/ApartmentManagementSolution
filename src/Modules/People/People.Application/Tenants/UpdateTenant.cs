using FluentValidation;
using MediatR;
using People.Application.Tenants.Commands.Update;
using People.Domain.Abstraction;
using People.Domain.Entities;
using People.Domain.ValueObjects;

namespace People.Application.Tenants
{
    internal class UpdateTenant(ITenantRepository repo, IValidator<UpdateTenantCommand> validator) : IRequestHandler<UpdateTenantCommand, bool>
    {
        private readonly ITenantRepository _repo = repo;
        private readonly IValidator<UpdateTenantCommand> _validator = validator;

        public async Task<bool> Handle(UpdateTenantCommand c, CancellationToken ct)
        {
            var result = await _validator.ValidateAsync(c, ct);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            var existingTenant = await _repo.GetByIdAsync(new TenantId(c.Id), ct);

            existingTenant.ChangeName(new PersonName(c.FirstName, c.LastName));
            existingTenant.ChangeEmail(new Email(c.Email));
            existingTenant.ChangePhone(string.IsNullOrWhiteSpace(c.Phone) ? null : new Phone(c.Phone));
            existingTenant.UpdateNotes(c.Notes);

            await _repo.UpdateAsync(existingTenant, ct);
            await _repo.SaveChangesAsync(ct);
            return true;
        }
    }
}
