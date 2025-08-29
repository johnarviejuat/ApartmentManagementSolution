using AutoMapper;
using Billing.Application.Common;
using Billing.Domain.Entities;

namespace Billing.Application.Payments.Mapping;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<Payment, PaymentDto>()
            .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
            .ForCtorParam("Amount", o => o.MapFrom(s => s.Amount))
            .ForCtorParam("PaidAt", o => o.MapFrom(s => DateOnly.FromDateTime(s.PaidAt)))
            .ForCtorParam("Method", o => o.MapFrom(s => s.Method))
            .ForCtorParam("ReferenceNumber", o => o.MapFrom(s => s.ReferenceNumber ?? string.Empty))
            .ForCtorParam("Notes", o => o.MapFrom(s => s.Notes ?? string.Empty))
            .ForMember(d => d.DepositPortion, o => o.MapFrom(s => s.DepositPortion));
    }
}
