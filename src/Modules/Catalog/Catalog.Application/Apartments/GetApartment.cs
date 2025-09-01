using AutoMapper;
using Catalog.Application.Common;
using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using MediatR;

using People.Domain.Abstraction;
using People.Domain.Entities;

namespace Catalog.Application.Apartments;

public record GetApartmentQuery(Guid Id) : IRequest<ApartmentDto?>;
public record GetAllApartmentsQuery : IRequest<IEnumerable<ApartmentDto>>;

public class GetApartmentHandler(IApartmentRepository repo, IOwnerRepository ownerRepo, ITenantRepository tenantRepo, IMapper mapper) : IRequestHandler<GetApartmentQuery, ApartmentDto?>
{
    private readonly IApartmentRepository _repo = repo;
    private readonly IOwnerRepository _ownerRepo = ownerRepo;
    private readonly ITenantRepository _tenantRepo = tenantRepo;
    private readonly IMapper _mapper = mapper;

    public async Task<ApartmentDto?> Handle(GetApartmentQuery q, CancellationToken ct)
    {
        var apartment = await _repo.GetByIdAsync(new ApartmentId(q.Id), ct);
        if (apartment is null) return null;

        PersonInformation? owner = null;
        if (apartment.OwnerId is not null)
        {
            var ownerEntity = await _ownerRepo.GetByIdAsync(new OwnerId(apartment.OwnerId.Value), ct);
            if (ownerEntity is not null)
                owner = _mapper.Map<PersonInformation>(ownerEntity);
        }

        PersonInformation[] tenants = [];
        if (apartment.Id is not null) 
        {
            var tenantEntities = await _tenantRepo.ListByApartmentIdAsync(apartment.Id.Value, ct);
            tenants = [.. tenantEntities.Select(t => _mapper.Map<PersonInformation>(t))];
        }

        var apartmentDto = _mapper.Map<ApartmentDto>(apartment) with { Owner = owner, Tenants = tenants};
        return apartmentDto;
    }
}

public class GetAllApartmentsHandler(IApartmentRepository repo, IOwnerRepository ownerRepo, ITenantRepository tenantRepo, IMapper mapper) : IRequestHandler<GetAllApartmentsQuery, IEnumerable<ApartmentDto>>
{
    private readonly IApartmentRepository _repo = repo;
    private readonly IOwnerRepository _ownerRepo = ownerRepo;
    private readonly ITenantRepository _tenantRepo = tenantRepo;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<ApartmentDto>> Handle(GetAllApartmentsQuery q, CancellationToken ct)
    {
        var apartments = await _repo.GetAllAsync(ct);
        var list = new List<ApartmentDto>(apartments.Count);

        foreach (var apartment in apartments)
        {
            PersonInformation? owner = null;
            if (apartment.OwnerId is not null)
            {
                var ownerEntity = await _ownerRepo.GetByIdAsync(new OwnerId(apartment.OwnerId.Value), ct);
                if (ownerEntity is not null)
                    owner = _mapper.Map<PersonInformation>(ownerEntity);
            }

            PersonInformation[] tenants = [];
            if (apartment.Id is not null)
            {
                var tenantEntities = await _tenantRepo.ListByApartmentIdAsync(apartment.Id.Value, ct);
                tenants = [.. tenantEntities.Select(t => _mapper.Map<PersonInformation>(t))];
            }
            var apartmentDto = _mapper.Map<ApartmentDto>(apartment) with {Owner = owner, Tenants = tenants };
            list.Add(apartmentDto);
        }
        return list;
    }
}
