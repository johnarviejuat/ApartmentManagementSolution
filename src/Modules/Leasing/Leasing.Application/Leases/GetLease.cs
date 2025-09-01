
using AutoMapper;
using MediatR;

using Leasing.Application.Common;
using Leasing.Domain.Abstraction;
using Leasing.Domain.Entities;

namespace Leasing.Application.Leases;

public sealed record GetLeaseAgreementQuery(Guid Id) : IRequest<LeaseDto?>;
public sealed class GetLeaseAgreementHandler(ILeaseRepository repo,IMapper mapper) : IRequestHandler<GetLeaseAgreementQuery, LeaseDto?>
{ 
    private readonly ILeaseRepository _repo = repo;
    private readonly IMapper _mapper = mapper;

    public async Task<LeaseDto?> Handle(GetLeaseAgreementQuery q, CancellationToken ct)
    {
        var lease = await _repo.GetByIdAsync(new LeaseId(q.Id), ct);
        if (lease is null) return null;

        return _mapper.Map<LeaseDto>(lease);
    }
}

public sealed record GetAllLeaseQuery() : IRequest<IEnumerable<LeaseDto>>;
public sealed class GetAllLeaseHandler(ILeaseRepository repo,IMapper mapper) : IRequestHandler<GetAllLeaseQuery, IEnumerable<LeaseDto>>
{ 
    private readonly ILeaseRepository _repo = repo;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<LeaseDto>> Handle(GetAllLeaseQuery q, CancellationToken ct)
    {
        var leases = await _repo.GetAllAsync(ct);
        return _mapper.Map<List<LeaseDto>>(leases);
    }
}