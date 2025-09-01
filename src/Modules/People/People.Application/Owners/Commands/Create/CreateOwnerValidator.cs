using FluentValidation;

namespace People.Application.Owners.Commands.Create;

public sealed class CreateOwnerValidator : AbstractValidator<CreateOwnerCommand>
{
    public CreateOwnerValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Phone).MaximumLength(40).When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.MailingLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MailingCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MailingState).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MailingPostalCode).NotEmpty().MaximumLength(20);
        
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}