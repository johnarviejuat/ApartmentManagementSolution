using AutoMapper;
using People.Application.Common;
using People.Domain.Entities;

namespace People.Application.Tenants.Mapping
{
    public class TenantMappingProfile : Profile
    {
        public TenantMappingProfile()
        {
            CreateMap<Tenant, TenantDto>()
                .ForCtorParam("Id", o => o.MapFrom(s => s.Id.Value))
                .ForCtorParam("FirstName", o => o.MapFrom(s => s.Name.First))
                .ForCtorParam("LastName", o => o.MapFrom(s => s.Name.Last))
                .ForCtorParam("Email", o => o.MapFrom(s => s.Email.Value))
                .ForCtorParam("Phone", o => o.MapFrom(s => s.Phone == null ? null : s.Phone.Value))
                .ForCtorParam("ApartmentId", o => o.MapFrom(s => s.ApartmentId == null ? (Guid?)null : s.ApartmentId.Value))
                .ForCtorParam("MoveInDate", o => o.MapFrom(s => s.MoveInDate))
                .ForCtorParam("MoveOutDate", o => o.MapFrom(s => s.MoveOutDate))
                .ForCtorParam("IsActive", o => o.MapFrom(s => s.Status == TenantStatus.Active))
                .ForCtorParam("CreatedAt", o => o.MapFrom(s => s.CreatedAt))
                .ForCtorParam("UpdatedAt", o => o.MapFrom(s => s.UpdatedAt));
        }
    }
}
