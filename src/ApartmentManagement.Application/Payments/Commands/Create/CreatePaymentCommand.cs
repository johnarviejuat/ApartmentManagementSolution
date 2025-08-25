
using MediatR;

namespace ApartmentManagement.Application.Payments.Commands.Create
{
    public sealed record CreatePaymentCommand(
        Guid TenantId,
        decimal Amount,
        string Method,
        Guid? ApartmentId,
        string? Notes
    ) : IRequest<Guid>;
}
