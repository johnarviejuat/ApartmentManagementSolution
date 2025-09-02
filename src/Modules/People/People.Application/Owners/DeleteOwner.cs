using FluentValidation;
using MediatR;

using People.Application.Owners.Commands.Delete;
using People.Domain.Abstraction;
using People.Domain.Entities;

namespace People.Application.Owners
{
    public sealed class DeleteOwner(IOwnerRepository repo, IValidator<DeleteOwnerCommand> validator) : IRequestHandler<DeleteOwnerCommand, bool>
    {
        public readonly IOwnerRepository _repo = repo;
        private readonly IValidator<DeleteOwnerCommand> _validator = validator;
        public async Task<bool> Handle(DeleteOwnerCommand c, CancellationToken ct)
        {
            var result = await _validator.ValidateAsync(c, ct);
            if (!result.IsValid) throw new ValidationException(result.Errors);

            var owner = await _repo.GetByIdAsync(new OwnerId(c.Id), ct);

            if (owner is null)
                return false;

            if (owner.IsDeleted)
                return false;

            owner.SoftDelete();

            await _repo.UpdateAsync(owner, ct);
            await _repo.SaveChangesAsync(ct);
            return true;

        }
    }
}
