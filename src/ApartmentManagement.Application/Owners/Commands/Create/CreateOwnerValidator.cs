using FluentValidation;

namespace ApartmentManagement.Application.Owners.Commands.Create;

public sealed class CreateOwnerValidator : AbstractValidator<CreateOwnerCommand>
{
    public CreateOwnerValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Phone).MaximumLength(40).When(x => !string.IsNullOrWhiteSpace(x.Phone));

        When(x => x.MailingAddress is not null, () =>
        {
            RuleFor(x => x.MailingAddress!.Line1).NotEmpty().MaximumLength(200);
            RuleFor(x => x.MailingAddress!.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.MailingAddress!.State).NotEmpty().MaximumLength(100);
            RuleFor(x => x.MailingAddress!.PostalCode).NotEmpty().MaximumLength(20);
        });

        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}