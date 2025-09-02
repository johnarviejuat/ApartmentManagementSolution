using FluentValidation;
using MediatR;

using People.Application.Tenants.Commands.Delete;
using People.Domain.Abstraction;
using People.Domain.Entities;

namespace People.Application.Tenants
{
    public sealed class DeletDeleteTenanteOwner(ITenantRepository repo, IValidator<DeleteTenantCommand> validator) : IRequestHandler<DeleteTenantCommand, bool>
    {
        private readonly ITenantRepository _repo = repo;
        private readonly IValidator<DeleteTenantCommand> _validator = validator;

        public async Task<bool> Handle(DeleteTenantCommand c, CancellationToken ct)
        {
            var result = await _validator.ValidateAsync(c, ct);
            if (!result.IsValid) throw new ValidationException(result.Errors);

            var tenant = await _repo.GetByIdAsync(new TenantId(c.Id), ct);

            if (tenant is null)
                return false;

            if (tenant.IsDeleted)
                return false;

            tenant.SoftDelete();

            await _repo.UpdateAsync(tenant, ct);
            await _repo.SaveChangesAsync(ct);

            return true;
        }
    }
}
