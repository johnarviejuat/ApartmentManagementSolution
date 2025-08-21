using MediatR;

namespace ApartmentManagement.Application.Tenants.Commands.Create;

public sealed record CreateTenantCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    DateOnly? MoveInDate,
    DateOnly? MoveOutDate,
    string? Notes,
    Guid? ApartmentId
) : IRequest<Guid>;
