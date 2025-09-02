

using MediatR;

namespace People.Application.Owners.Commands.Update
{
    public sealed record UpdateOwnerCommand(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string? Phone,
        string? MailingLine1,
        string? MailingCity,
        string? MailingState,
        string? MailingPostalCode,
        string? Notes
    ) : IRequest<bool>;
}

