using ApartmentManagement.Application.Common;
using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Leases;
using ApartmentManagement.Domain.Leasing.Owners;
using ApartmentManagement.Domain.Leasing.Tenants;
using AutoMapper;
using MediatR;

namespace ApartmentManagement.Application.Leases;

public sealed record GetAllLeaseQuery() : IRequest<IEnumerable<LeaseDto>>;
public sealed record GetLeaseAgreementQuery(Guid Id) : IRequest<LeaseDto?>;

public class GetLeaseAgreementHandler(
    ILeaseRepository repo, 
    IApartmentRepository apartmentRepo,
    IOwnerRepository ownerRepo,
    ITenantRepository tenantRepo,
    IMapper mapper
    ) : IRequestHandler<GetLeaseAgreementQuery, LeaseDto?>
{ 
    private readonly ILeaseRepository _repo = repo;
    private readonly IApartmentRepository _apartmentRepo = apartmentRepo;
    private readonly IOwnerRepository _ownerRepo = ownerRepo;
    private readonly ITenantRepository _tenantRepo = tenantRepo;
    private readonly IMapper _mapper = mapper;
    public async Task<LeaseDto?> Handle(GetLeaseAgreementQuery q, CancellationToken ct)
    {
        var lease = await _repo.GetByIdAsync((q.Id), ct);
        if (lease is null) return null;

        ApartmentDto? apartmentDto = null;
        if (lease.ApartmentId is not null)
        {
            var apartment = await _apartmentRepo.GetByIdAsync(lease.ApartmentId, ct);
            if (apartment is null) return null;

            OwnerDto? ownerDto = null;
            if (apartment.OwnerId is not null)
            {
                var owner = await _ownerRepo.GetByIdAsync(apartment.OwnerId, ct);
                if (owner is not null)
                    ownerDto = _mapper.Map<OwnerDto>(owner);
            }

            var tenants = await _tenantRepo.ListByApartmentIdAsync(apartment.Id, ct);
            var tenantDtos = tenants is { Count: > 0 }
                ? _mapper.Map<IReadOnlyList<TenantDto>>(tenants)
                : [];

            apartmentDto = _mapper.Map<ApartmentDto>(apartment) with { Owner = ownerDto, Tenants = tenantDtos };
        }
        return _mapper.Map<LeaseDto>(lease) with {  Apartment = apartmentDto };
    }
}

public sealed class GetAllLeaseHandler(
    ILeaseRepository repo, 
    IMapper mapper
    ) : IRequestHandler<GetAllLeaseQuery, IEnumerable<LeaseDto>>
{ 
    private readonly ILeaseRepository _repo = repo;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<LeaseDto>> Handle(GetAllLeaseQuery q, CancellationToken ct)
    {
        var leases = await _repo.GetAllAsync(ct);
        return _mapper.Map<List<LeaseDto>>(leases);
    }
}