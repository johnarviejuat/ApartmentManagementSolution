using ApartmentManagement.Application.Common;
using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Tenants;
using MediatR;
using AutoMapper;
using ApartmentManagement.Domain.Leasing.Leases;
using ApartmentManagement.Domain.Leasing.Owners;

namespace ApartmentManagement.Application.Tenants;

public sealed record GetTenantQuery(Guid Id) : IRequest<TenantDto?>;

public sealed class GetTenantHandler(ITenantRepository repo, IApartmentRepository apartment_repo, ILeaseRepository lease_repo, IMapper mapper)
    : IRequestHandler<GetTenantQuery, TenantDto?>
{
    private readonly ITenantRepository _repo = repo;
    private readonly IApartmentRepository _apartment_repo = apartment_repo;
    private readonly ILeaseRepository _lease_repo = lease_repo;
    private readonly IMapper _mapper = mapper;

    public async Task<TenantDto?> Handle(GetTenantQuery q, CancellationToken ct)
    {
        var tenant = await _repo.GetByIdAsync(new TenantId(q.Id), ct);
        if (tenant is null) return null;


        var apartmentName = string.Empty;
        if (tenant.ApartmentId is not null)
        {
            var apartment = await _apartment_repo.GetByIdAsync(tenant.ApartmentId, ct);
            if (apartment is not null)
            {
                apartmentName = apartment.Name + " - " + "Unit " + apartment.UnitNumber;
            }
        }

        LeaseDto? leaseDto = null;
        if (tenant.ApartmentId is not null)
        {
            var lease = await _lease_repo.GetActiveAsync(tenant.Id, tenant.ApartmentId, ct);
            if (lease is not null) 
            { 
                leaseDto = _mapper.Map<LeaseDto>(lease);
            }
        }

        var tenantDto = _mapper.Map<TenantDto>(tenant);
        tenantDto = tenantDto with { Apartment = apartmentName, Lease = leaseDto };
        return tenantDto;
    }
}

