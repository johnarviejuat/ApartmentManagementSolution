using FluentValidation;

using People.Domain.Abstraction;
using People.Domain.Entities;

namespace People.Application.Tenants.Commands.Delete
{
    public sealed class DeleteTenantValidator : AbstractValidator<DeleteTenantCommand>
    {
        private readonly ITenantRepository _tenantRepo;
        public DeleteTenantValidator(ITenantRepository tenantRepo)
        {
            _tenantRepo = tenantRepo;

            RuleFor(x => x.Id).NotEmpty().WithMessage("Tenant Id must be provided.");
            RuleFor(c => c.Id)
                .MustAsync(async (id, ct) =>
                    !await _tenantRepo.AnyActiveByTenantId(new TenantId(id), ct))
                .WithMessage("Tenant cannot be deleted because they are currently assigned to an apartment.");
        }
    }
}
