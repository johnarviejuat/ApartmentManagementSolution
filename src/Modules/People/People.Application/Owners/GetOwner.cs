using AutoMapper;
using Catalog.Domain.Entities;
using MediatR;
using People.Application.Common;
using People.Domain.Abstraction;
using People.Domain.Entities;

namespace People.Application.Owners;
public sealed record GetOwnerQuery(Guid Id) : IRequest<OwnerDto?>;

public sealed class GetOwnerHandler(IOwnerRepository repo, IMapper mapper) : IRequestHandler<GetOwnerQuery, OwnerDto?>
{
    private readonly IOwnerRepository _repo = repo;
    private readonly IMapper _mapper = mapper;

    public async Task<OwnerDto?> Handle(GetOwnerQuery q, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(new OwnerId(q.Id), ct);
        return entity is null ? null : _mapper.Map<OwnerDto>(entity);
    }
}

public sealed record GetAllOwnerQuery : IRequest<IEnumerable<OwnerDto>>;

public class GetAllOwnerHandler(IOwnerRepository repo, IMapper mapper) : IRequestHandler<GetAllOwnerQuery, IEnumerable<OwnerDto>>
{
    private readonly IOwnerRepository _repo = repo;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<OwnerDto>> Handle(GetAllOwnerQuery request, CancellationToken ct)
    {
        var owners = await _repo.GetAllAsync(ct);
        var list = new List<OwnerDto>(owners.Count);
        foreach (var owner in owners)
        {
            var ownerDto = _mapper.Map<OwnerDto>(owner);
            list.Add(ownerDto);
        }
        return list;
    }
}
