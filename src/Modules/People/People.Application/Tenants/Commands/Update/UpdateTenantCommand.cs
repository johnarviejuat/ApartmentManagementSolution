using MediatR;

namespace People.Application.Tenants.Commands.Update
{
    public sealed record UpdateTenantCommand(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string? Phone,
        string? Notes = ""
    ) : IRequest<bool>;
}
