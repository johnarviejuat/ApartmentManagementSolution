using FluentValidation;
using MediatR;
using People.Application.Owners.Commands.Create;
using People.Domain.Abstraction;
using People.Domain.Entities;
using People.Domain.ValueObjects;

namespace People.Application.Owners;

public sealed class CreateOwnerHandler(IOwnerRepository repo, IValidator<CreateOwnerCommand> validator)
        : IRequestHandler<CreateOwnerCommand, Guid>
{
    private readonly IOwnerRepository _repo = repo;
    private readonly IValidator<CreateOwnerCommand> _validator = validator;

    public async Task<Guid> Handle(CreateOwnerCommand c, CancellationToken ct)
    {
        var result = await _validator.ValidateAsync(c, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);


        var entity = new Owner(
            new OwnerId(Guid.NewGuid()),
            new PersonName(c.FirstName, c.LastName),
            new Email(c.Email),
            string.IsNullOrWhiteSpace(c.Phone) ? null : new Phone(c.Phone),
            new Address(c.MailingLine1, c.MailingCity, c.MailingState, c.MailingPostalCode),
            c.Notes);

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return entity.Id.Value;
    }
}
