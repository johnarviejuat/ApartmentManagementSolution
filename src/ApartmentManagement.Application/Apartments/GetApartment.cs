using ApartmentManagement.Application.Common;
using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Owners;
using AutoMapper;
using MediatR;

namespace ApartmentManagement.Application.Apartments;

public record GetApartmentQuery(Guid Id) : IRequest<ApartmentDto?>;
public record GetAllApartmentsQuery : IRequest<IEnumerable<ApartmentDto>>;

public class GetApartmentHandler(IApartmentRepository repo, IOwnerRepository ownerRepo, IMapper mapper) : IRequestHandler<GetApartmentQuery, ApartmentDto?>
{
    private readonly IApartmentRepository _repo = repo;
    private readonly IOwnerRepository _ownerRepo = ownerRepo;
    private readonly IMapper _mapper = mapper;

    public async Task<ApartmentDto?> Handle(GetApartmentQuery q, CancellationToken ct)
    {
        var apartment = await _repo.GetByIdAsync(new ApartmentId(q.Id), ct);
        if (apartment is null) return null;

        OwnerDto? ownerDto = null;
        if (apartment.OwnerId is not null)
        {
            var owner = await _ownerRepo.GetByIdAsync(apartment.OwnerId, ct);
            if (owner is not null)
                ownerDto = _mapper.Map<OwnerDto>(owner);
        }

        var apartmentDto = _mapper.Map<ApartmentDto>(apartment);
        apartmentDto = apartmentDto with { Owner = ownerDto };

        return apartmentDto;
    }
}

public class GetAllApartmentsHandler(IApartmentRepository repo, IOwnerRepository ownerRepo, IMapper mapper) : IRequestHandler<GetAllApartmentsQuery, IEnumerable<ApartmentDto>>
{
    private readonly IApartmentRepository _repo = repo;
    private readonly IOwnerRepository _ownerRepo = ownerRepo;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<ApartmentDto>> Handle(GetAllApartmentsQuery q, CancellationToken ct)
    {
        var apartments = await _repo.GetAllAsync(ct);
        var list = new List<ApartmentDto>(apartments.Count);


        foreach (var apartment in apartments)
        {

            OwnerDto? ownerDto = null;
            if (apartment.OwnerId is not null)
            {
                var owner = await _ownerRepo.GetByIdAsync(apartment.OwnerId, ct);
                if (owner is not null)
                    ownerDto = _mapper.Map<OwnerDto>(owner);
            }

            var apartmentDto = _mapper.Map<ApartmentDto>(apartment);
            apartmentDto = apartmentDto with { Owner = ownerDto };
            list.Add(apartmentDto);
        }

        return list;
    }
}
