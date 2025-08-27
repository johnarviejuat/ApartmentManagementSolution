using ApartmentManagement.Application.Common;
using ApartmentManagement.Domain.Leasing.Leases;
using AutoMapper;
namespace ApartmentManagement.Application.Leases.Mapping
{
    public class LeaseMappingProfile : Profile
    {
        public LeaseMappingProfile() {
            CreateMap<Lease, LeaseDto>()
               .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
               .ForCtorParam("MonthlyRent", opt => opt.MapFrom(src => src.MonthlyRent))
               .ForCtorParam("NextDueDate", opt => opt.MapFrom(src => src.NextDueDate))
               .ForCtorParam("Credit", opt => opt.MapFrom(src => src.Credit))
               .ForCtorParam("DepositRequired", opt => opt.MapFrom(src => src.DepositRequired))
               .ForCtorParam("DepositHeld", opt => opt.MapFrom(src => src.DepositHeld))
               .ForCtorParam("IsDepositFunded", opt => opt.MapFrom(src => src.IsDepositFunded));
        }
    }
}
