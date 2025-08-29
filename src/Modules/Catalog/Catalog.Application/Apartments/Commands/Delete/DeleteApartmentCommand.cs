using MediatR;

namespace Catalog.Application.Apartments.Commands.Delete;

public sealed record DeleteApartmentCommand(Guid Id) : IRequest<bool>;
