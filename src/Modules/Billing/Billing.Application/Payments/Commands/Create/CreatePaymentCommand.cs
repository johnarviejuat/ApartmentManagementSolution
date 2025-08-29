using Billing.Application.Common;
using MediatR;

namespace Billing.Application.Payments.Commands.Create
{
    public sealed record CreatePaymentCommand(
        TenantRefId TenantId,
        ApartmentRefId ApartmentId,
        decimal Amount,
        string Method,
        string? Notes = ""
    ) : IRequest<Guid>;
}
