using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Tenants;
using MediatR;

namespace ApartmentManagement.Application.Payments.Commands.Create
{
    public sealed record CreatePaymentCommand(
        TenantId TenantId,
        ApartmentId ApartmentId,
        decimal Amount,
        string Method,
        string? Notes = ""
    ) : IRequest<Guid>;
}
