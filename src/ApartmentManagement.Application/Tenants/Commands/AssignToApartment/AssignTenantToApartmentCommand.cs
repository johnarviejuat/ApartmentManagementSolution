using MediatR;

namespace ApartmentManagement.Application.Tenants.Commands.AssignToApartment;

public sealed record AssignTenantToApartmentCommand(
    Guid TenantId,
    Guid ApartmentId
) : IRequest<Unit>;
