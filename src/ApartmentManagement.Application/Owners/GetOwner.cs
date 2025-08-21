using ApartmentManagement.Application.Common;
using ApartmentManagement.Domain.Leasing.Owners;
using MediatR;
using AutoMapper;

namespace ApartmentManagement.Application.Owners;
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
