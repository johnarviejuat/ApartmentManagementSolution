using MediatR;
using People.Domain.Entities;

namespace People.Application.Tenants.Commands.RemoveToApartment;
public sealed record RemoveTenantFromApartmentCommand(Guid TenantId, Guid ApartmentId, TenantStatus TenantStatus) : IRequest<bool>;

