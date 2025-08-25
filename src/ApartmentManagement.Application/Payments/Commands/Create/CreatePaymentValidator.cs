using FluentValidation;

namespace ApartmentManagement.Application.Payments.Commands.Create
{
    public sealed class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentValidator()
        {
            RuleFor(x => x.TenantId)
                .NotNull().WithMessage("TenantId is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Method)
                .NotEmpty().WithMessage("Payment method is required.")
                .MaximumLength(50).WithMessage("Payment method must not exceed 50 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).When(x => !string.IsNullOrWhiteSpace(x.Notes));
        }
    }
}
