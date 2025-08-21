using MediatR;

namespace ApartmentManagement.Application.Apartments.Commands.AssignOwner;

public sealed record AssignOwnerToApartmentCommand(Guid OwnerId, Guid ApartmentId) : IRequest<Unit>;
