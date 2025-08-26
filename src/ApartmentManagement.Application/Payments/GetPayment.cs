using ApartmentManagement.Application.Common;
using ApartmentManagement.Application.Tenants;
using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Payments;
using ApartmentManagement.Domain.Leasing.Tenants;
using AutoMapper;
using MediatR;

namespace ApartmentManagement.Application.Payments;

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

        ApartmentDto? apartmentDto = null;
        if (payment.ApartmentId is not null)
        {
            var apartment = await _apartmentRepo.GetByIdAsync(payment.ApartmentId, ct);
            if (apartment is not null)
                apartmentDto = _mapper.Map<ApartmentDto>(apartment);
        }

        TenantDto? tenantDto = null;
        if (payment.TenantId is not null)
        {
            var tenant = await _tenantRepo.GetByIdAsync(payment.TenantId, ct);
            if (tenant is not null)
                tenantDto = _mapper.Map<TenantDto>(tenant);
        }

        if (payment is null) return null;
        var paymentDto = _mapper.Map<PaymentDto>(payment) with { Apartment = apartmentDto, Tenant = tenantDto };

        return paymentDto;
    }
}
