using ApartmentManagement.Application.Owners.Commands.Create;
using ApartmentManagement.Domain.Leasing.Owners;
using FluentValidation;
using MediatR;

public sealed class CreateOwnerHandler(IOwnerRepository repo, IValidator<CreateOwnerCommand> validator): IRequestHandler<CreateOwnerCommand, Guid>
{
    private readonly IOwnerRepository _repo = repo;
    private readonly IValidator<CreateOwnerCommand> _validator = validator;

    public async Task<Guid> Handle(CreateOwnerCommand c, CancellationToken ct)
    {
        var result = await _validator.ValidateAsync(c, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        ApartmentManagement.Domain.Leasing.Apartments.Address? mailing =
            c.MailingAddress is null
                ? null
                : new ApartmentManagement.Domain.Leasing.Apartments.Address(
                    c.MailingAddress.Line1,
                    c.MailingAddress.City,
                    c.MailingAddress.State,
                    c.MailingAddress.PostalCode
                  );

        var entity = new Owner(
            new OwnerId(Guid.NewGuid()),
            new PersonName(c.FirstName, c.LastName),
            new Email(c.Email),
            string.IsNullOrWhiteSpace(c.Phone) ? null : new Phone(c.Phone),
            mailing,
            c.Notes
        );

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return entity.Id.Value;
    }
}
