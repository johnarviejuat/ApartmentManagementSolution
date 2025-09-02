using FluentValidation;
using MediatR;
using People.Application.Owners.Commands.Update;
using People.Domain.Abstraction;
using People.Domain.Entities;
using People.Domain.ValueObjects;

namespace People.Application.Owners
{
    public sealed class UpdateOwner(IOwnerRepository repo, IValidator<UpdateOwnerCommand> validator) : IRequestHandler<UpdateOwnerCommand, bool>
    {
        private readonly IOwnerRepository _repo = repo;
        private readonly IValidator<UpdateOwnerCommand> _validator = validator;

        public async Task<bool> Handle(UpdateOwnerCommand c, CancellationToken ct)
        {
            var result = await _validator.ValidateAsync(c, ct);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            var existingOwner = await _repo.GetByIdAsync(new OwnerId(c.Id), ct);

            existingOwner.ChangeName(new PersonName(c.FirstName, c.LastName));
            existingOwner.ChangeEmail(new Email(c.Email));
            existingOwner.ChangePhone(string.IsNullOrWhiteSpace(c.Phone) ? null : new Phone(c.Phone));
            existingOwner.ChangeMailingAddress(new Address(c.MailingLine1, c.MailingCity, c.MailingState, c.MailingPostalCode));
            existingOwner.UpdateNotes(c.Notes);

            await _repo.UpdateAsync(existingOwner, ct);
            await _repo.SaveChangesAsync(ct);
            return true;
        }
    }
}
