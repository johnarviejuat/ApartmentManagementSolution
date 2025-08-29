using MediatR;
using People.Application.Common;

namespace People.Application.Owners.Commands.Create;

public sealed record CreateOwnerCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    AddressDto? MailingAddress,
    string? Notes
) : IRequest<Guid>;
