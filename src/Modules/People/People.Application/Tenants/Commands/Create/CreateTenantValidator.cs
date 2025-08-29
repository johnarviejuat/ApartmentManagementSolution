using FluentValidation;

namespace People.Application.Tenants.Commands.Create;

public sealed class CreateTenantValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(320)
            .EmailAddress();

        RuleFor(x => x.Phone)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));

        RuleFor(x => x.MoveOutDate)
            .Must((cmd, moveOut) => moveOut == null || cmd.MoveInDate == null || moveOut >= cmd.MoveInDate)
            .WithMessage("Move-out date cannot be earlier than move-in date.");

        // optional apartment id sanity
        RuleFor(x => x.ApartmentId)
            .NotEqual(Guid.Empty)
            .When(x => x.ApartmentId.HasValue);
    }
}
