using ApartmentManagement.Domain.Leasing.Tenants;
using MediatR;

namespace ApartmentManagement.Application.Tenants.Commands.RemoveToApartment;
public sealed record RemoveTenantFromApartmentCommand(Guid TenantId, Guid ApartmentId, TenantStatus TenantStatus) : IRequest<bool>;

