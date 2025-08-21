using FluentValidation;

namespace ApartmentManagement.Application.Apartments.Commands.AssignOwner;

public sealed class AssignOwnerToApartmentCommandValidator
    : AbstractValidator<AssignOwnerToApartmentCommand>
{
    public AssignOwnerToApartmentCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.ApartmentId).NotEmpty();
    }
}
