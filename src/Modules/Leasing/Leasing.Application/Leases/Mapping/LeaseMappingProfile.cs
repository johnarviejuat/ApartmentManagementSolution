using AutoMapper;
using Leasing.Application.Common;
using Leasing.Domain.Entities;
namespace Leasing.Application.Leases.Mapping
{
    public class LeaseMappingProfile : Profile
    {
        public LeaseMappingProfile() {

            CreateMap<Lease, LeaseDto>()
                .ForCtorParam(nameof(LeaseDto.Id), o => o.MapFrom(s => s.Id.Value))
                .ForCtorParam(nameof(LeaseDto.MonthlyRent), o => o.MapFrom(s => s.MonthlyRent))
                .ForCtorParam(nameof(LeaseDto.NextDueDate), o => o.MapFrom(s => s.NextDueDate))
                .ForCtorParam(nameof(LeaseDto.Credit), o => o.MapFrom(s => s.Credit))
                .ForCtorParam(nameof(LeaseDto.DepositRequired), o => o.MapFrom(s => s.DepositRequired))
                .ForCtorParam(nameof(LeaseDto.DepositHeld), o => o.MapFrom(s => s.DepositHeld))
                .ForCtorParam(nameof(LeaseDto.IsDepositFunded), o => o.MapFrom(s => s.IsDepositFunded))
                .ForCtorParam(nameof(LeaseDto.IsActive), o => o.MapFrom(s => s.IsActive));        
        }
    }
}
