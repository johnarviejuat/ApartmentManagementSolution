using MediatR;

namespace Catalog.Application.Apartments.Commands.AssignOwner;

public sealed record AssignOwnerToApartmentCommand(Guid OwnerId, Guid ApartmentId) : IRequest<Unit>;
