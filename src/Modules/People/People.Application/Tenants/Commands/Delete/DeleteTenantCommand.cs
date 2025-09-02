using MediatR;

namespace People.Application.Tenants.Commands.Delete;

public sealed record DeleteTenantCommand(Guid Id) : IRequest<bool>;
