using MediatR;

namespace People.Application.Owners.Commands.Delete;

public sealed record DeleteOwnerCommand(Guid Id) : IRequest<bool>;

