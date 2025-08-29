using AutoMapper;
using Billing.Application.Common;
using Billing.Domain.Abstraction;
using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using MediatR;
using People.Domain.Abstraction;
using People.Domain.Entities;

namespace Billing.Application.Payments;

public sealed record GetPaymentQuery(Guid Id) : IRequest<PaymentDto?>;

public sealed class GetPaymentHandler(
    IPaymentRepository repo, 
    IApartmentRepository apartmentRepo,
    ITenantRepository tenantRepo,
    IMapper mapper
    ) : IRequestHandler<GetPaymentQuery, PaymentDto?>
{
    private readonly IPaymentRepository _repo = repo;
    private readonly IApartmentRepository _apartmentRepo = apartmentRepo;
    private readonly ITenantRepository _tenantRepo = tenantRepo;
    private readonly IMapper _mapper = mapper;

    public async Task<PaymentDto?> Handle(GetPaymentQuery q, CancellationToken ct)
    {
        var payment = await _repo.GetByIdAsync(q.Id, ct);

        var apartmentName = "";
        if (payment.ApartmentId is Guid)
        {
            var apartment = await _apartmentRepo.GetByIdAsync(new ApartmentId(payment.ApartmentId), ct);
            if (apartment is not null)
               apartmentName = apartment.Name + " - " + "Unit " + apartment.UnitNumber;
        }
        var tenantName = "";
        if (payment.TenantId is Guid)
        {
            var tenant = await _tenantRepo.GetByIdAsync(new TenantId(payment.TenantId), ct);
            if (tenant is not null)
               tenantName = tenant.Name.First + " " + tenant.Name.Last;
        }

        if (payment is null) return null;
        var paymentDto = _mapper.Map<PaymentDto>(payment) with { Apartment = apartmentName, Tenant = tenantName };

        return paymentDto;
    }
}
