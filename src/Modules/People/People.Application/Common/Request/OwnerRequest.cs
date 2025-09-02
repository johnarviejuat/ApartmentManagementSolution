

namespace People.Application.Common.Request
{ 
    public sealed record OwnerRequest(
        string FirstName,
        string LastName,
        string Email,
        string? Phone,
        string? MailingLine1,
        string? MailingCity,
        string? MailingState,
        string? MailingPostalCode,
        string? Notes

    );
}
