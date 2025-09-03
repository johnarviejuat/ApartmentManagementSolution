
using AutoMapper;
using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using Leasing.Application.Common;
using Leasing.Domain.Abstraction;
using Leasing.Domain.Entities;
using MediatR;
using People.Domain.Abstraction;
using People.Domain.Entities;

namespace Leasing.Application.Leases;

public sealed record GetLeaseAgreementQuery(Guid Id) : IRequest<LeaseDto?>;
public sealed class GetLeaseAgreementHandler(ILeaseRepository repo, IApartmentRepository apartmentRepo, IOwnerRepository ownerRepo, ITenantRepository tenantRepo, IMapper mapper) : IRequestHandler<GetLeaseAgreementQuery, LeaseDto?>
{ 
    private readonly ILeaseRepository _repo = repo;
    private readonly IApartmentRepository _apartmentRepo = apartmentRepo;
    private readonly IOwnerRepository _ownerRepo = ownerRepo;
    private readonly ITenantRepository _tenantRepo = tenantRepo;
    private readonly IMapper _mapper = mapper;

    public async Task<LeaseDto?> Handle(GetLeaseAgreementQuery q, CancellationToken ct)
    {
        var lease = await _repo.GetByIdAsync(new LeaseId(q.Id), ct);
        if (lease is null) return null;

        ApartmentInformation? apartment = null;
        PersonInformation? owner = null;
        PersonInformation? tenant = null;

        if (lease.ApartmentId != Guid.Empty)
        {
            var apartmentEntity = await _apartmentRepo.GetByIdAsync(new ApartmentId(lease.ApartmentId), ct);
            if (apartmentEntity is not null)
                apartment = _mapper.Map<ApartmentInformation>(apartmentEntity);


            if (apartmentEntity.OwnerId is not null)
            {
                var ownerEntity = await _ownerRepo.GetByIdAsync(new OwnerId(apartmentEntity.OwnerId.Value), ct);
                if (ownerEntity is not null)
                    owner = _mapper.Map<PersonInformation>(ownerEntity);

            }
        }

        if (lease.TenantId != Guid.Empty)
        {
            var tenantEntity = await _tenantRepo.GetByIdAsync(new TenantId(lease.TenantId), ct);
            if (tenantEntity is not null)
                tenant = _mapper.Map<PersonInformation>(tenantEntity);
        }

        return _mapper.Map<LeaseDto>(lease) with { Apartment = apartment, Owner = owner, Tenant = tenant };
    }
}

public sealed record GetAllLeaseQuery() : IRequest<IEnumerable<LeaseDto>>;
public sealed class GetAllLeaseHandler(ILeaseRepository repo, IApartmentRepository apartmentRepo, IOwnerRepository ownerRepo, ITenantRepository tenantRepo, IMapper mapper) : IRequestHandler<GetAllLeaseQuery, IEnumerable<LeaseDto>>
{ 
    private readonly ILeaseRepository _repo = repo;
    private readonly IApartmentRepository _apartmentRepo = apartmentRepo;
    private readonly IOwnerRepository _ownerRepo = ownerRepo;
    private readonly ITenantRepository _tenantRepo = tenantRepo;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<LeaseDto>> Handle(GetAllLeaseQuery q, CancellationToken ct)
    {
        var leases = await _repo.GetAllAsync(ct);
        var list = new List<LeaseDto>(leases.Count);

        foreach (var lease in leases)
        {
            ApartmentInformation? apartment = null;
            PersonInformation? owner = null;
            PersonInformation? tenant = null;

            if (lease.ApartmentId != Guid.Empty)
            {
                var apartmentEntity = await _apartmentRepo.GetByIdAsync(new ApartmentId(lease.ApartmentId), ct);
                if (apartmentEntity is not null)
                    apartment = _mapper.Map<ApartmentInformation>(apartmentEntity);

                
                if (apartmentEntity.OwnerId is not null)
                {
                    var ownerEntity = await _ownerRepo.GetByIdAsync(new OwnerId(apartmentEntity.OwnerId.Value), ct);
                    if (ownerEntity is not null)
                        owner = _mapper.Map<PersonInformation>(ownerEntity);

                }
            }

            if (lease.TenantId != Guid.Empty)
            {
                var tenantEntity = await _tenantRepo.GetByIdAsync(new TenantId(lease.TenantId), ct);
                if (tenantEntity is not null)
                    tenant = _mapper.Map<PersonInformation>(tenantEntity);
            }

            var leaseDto = _mapper.Map<LeaseDto>(lease) with { Apartment = apartment, Owner = owner, Tenant = tenant };
            list.Add(leaseDto);
        }

        return list;
    }
}