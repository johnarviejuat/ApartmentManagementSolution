

namespace People.Application.Common.Request
{
    public sealed record TenantRequest(
        string FirstName,
        string LastName,
        string Email,
        string? Phone,
        string? Notes = ""
    );
}
