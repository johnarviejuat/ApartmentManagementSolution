using ApartmentManagement.Application.Common;
using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Tenants;
using MediatR;
using AutoMapper;

namespace ApartmentManagement.Application.Tenants;

public sealed record GetTenantQuery(Guid Id) : IRequest<TenantDto?>;

public sealed class GetTenantHandler(ITenantRepository repo, IApartmentRepository apartment_repo, IMapper mapper)
    : IRequestHandler<GetTenantQuery, TenantDto?>
{
    private readonly ITenantRepository _repo = repo;
    private readonly IApartmentRepository _apartment_repo = apartment_repo;
    private readonly IMapper _mapper = mapper;

    public async Task<TenantDto?> Handle(GetTenantQuery q, CancellationToken ct)
    {
        var tenant = await _repo.GetByIdAsync(new TenantId(q.Id), ct);
        if (tenant is null) return null;

        ApartmentDto? apartmentDto = null;

        if (tenant.ApartmentId is not null)
        {
            var apartment = await _apartment_repo.GetByIdAsync(tenant.ApartmentId, ct);
            if (apartment is not null)
            {
                apartmentDto = _mapper.Map<ApartmentDto>(apartment);
            }
        }

        var tenantDto = _mapper.Map<TenantDto>(tenant);

        tenantDto = tenantDto with { Apartment = apartmentDto };

        return tenantDto;
    }
}

