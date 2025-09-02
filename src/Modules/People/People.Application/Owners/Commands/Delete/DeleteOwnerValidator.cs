using FluentValidation;

using Catalog.Domain.Abstractions;
using People.Domain.Entities;

namespace People.Application.Owners.Commands.Delete
{
    public sealed class DeleteOwnerValidator : AbstractValidator<DeleteOwnerCommand>
    {
        private readonly IApartmentRepository _apartmentRepo;

        public DeleteOwnerValidator(IApartmentRepository apartmentRepo)
        {
            _apartmentRepo = apartmentRepo;

            RuleFor(x => x.Id).NotEmpty().WithMessage("Owner Id must be provided.");
            RuleFor(c => c.Id)
                .MustAsync(async (id, ct) =>
                    !await apartmentRepo.AnyActiveByOwnerId(new OwnerId(id), ct))
                .WithMessage("Owner cannot be deleted because they still own apartments.");
        }
    }
}
