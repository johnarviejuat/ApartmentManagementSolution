using MediatR;

namespace People.Application.Tenants.Commands.AssignToApartment;

public sealed record AssignTenantToApartmentCommand(
    Guid TenantId,
    Guid ApartmentId
) : IRequest<Unit>;
