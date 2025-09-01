using MediatR;

namespace People.Application.Owners.Commands.Create;

public sealed record CreateOwnerCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? MailingLine1,
    string? MailingCity,
    string? MailingState,
    string? MailingPostalCode,
    string? Notes
) : IRequest<Guid>;
