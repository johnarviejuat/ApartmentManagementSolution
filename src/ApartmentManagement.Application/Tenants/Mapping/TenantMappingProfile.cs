using ApartmentManagement.Application.Common;
using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Tenants;
using AutoMapper;

namespace ApartmentManagement.Application.Tenants.Mapping
{
    public class TenantMappingProfile : Profile
    {
        public TenantMappingProfile()
        {

            CreateMap<TenantId, Guid>().ConvertUsing(id => id.Value);

            CreateMap<Tenant, TenantDto>()
                .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id.Value))
                .ForCtorParam("FirstName", opt => opt.MapFrom(src => src.Name.First))
                .ForCtorParam("LastName", opt => opt.MapFrom(src => src.Name.Last))
                .ForCtorParam("Email", opt => opt.MapFrom(src => src.Email.Value))
                .ForCtorParam("Phone", opt => opt.MapFrom(src => src.Phone.Value))
                .ForCtorParam("ApartmentId", opt => opt.MapFrom(src => src.ApartmentId.Value))
                .ForCtorParam("MoveInDate", opt => opt.MapFrom(src => src.MoveInDate))
                .ForCtorParam("MoveOutDate", opt => opt.MapFrom(src => src.MoveOutDate))
                .ForCtorParam("IsActive", opt => opt.MapFrom(src => src.Status == TenantStatus.Active))
                .ForCtorParam("CreatedAt", opt => opt.MapFrom(src => src.CreatedAt))
                .ForCtorParam("UpdatedAt", opt => opt.MapFrom(src => src.UpdatedAt));
        }
    }
}
