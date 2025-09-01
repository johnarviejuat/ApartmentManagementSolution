
using AutoMapper;
using MediatR;

using People.Application.Common;
using People.Domain.Abstraction;
using People.Domain.Entities;

using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;

namespace People.Application.Tenants;

public sealed record GetTenantQuery(Guid Id) : IRequest<TenantDto?>;

public sealed class GetTenantHandler(ITenantRepository repo, IApartmentRepository apartment_repo, IMapper mapper) : IRequestHandler<GetTenantQuery, TenantDto?>
{
    private readonly ITenantRepository _repo = repo;
    private readonly IApartmentRepository _apartment_repo = apartment_repo;
    private readonly IMapper _mapper = mapper;

    public async Task<TenantDto?> Handle(GetTenantQuery q, CancellationToken ct)
    {
        var tenant = await _repo.GetByIdAsync(new TenantId(q.Id), ct);
        if (tenant is null) return null;

        TenantApartment? apartment = null;
        if (tenant.ApartmentId is not null)
        {
            var apartmentEntity = await _apartment_repo.GetByIdAsync(new ApartmentId(tenant.ApartmentId.Value), ct);
            if (apartmentEntity is not null)
                apartment = _mapper.Map<TenantApartment>(apartmentEntity);
        }
        var tenantDto = _mapper.Map<TenantDto>(tenant) with {Apartment = apartment};
        return tenantDto;
    }
}

