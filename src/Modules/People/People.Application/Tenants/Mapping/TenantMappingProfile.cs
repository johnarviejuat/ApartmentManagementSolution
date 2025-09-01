using AutoMapper;
using Catalog.Domain.Entities;
using People.Application.Common;
using People.Domain.Entities;

namespace People.Application.Tenants.Mapping;
public class TenantMappingProfile : Profile
{
    public TenantMappingProfile()
    {
        CreateMap<TenantId, Guid>().ConvertUsing(id => id.Value);

        CreateMap<Tenant, TenantDto>()
            .ForMember(d => d.Id, m => m.MapFrom(s => s.Id.Value))
            .ForMember(d => d.FirstName, m => m.MapFrom(s => s.Name.First))
            .ForMember(d => d.LastName, m => m.MapFrom(s => s.Name.Last))
            .ForMember(d => d.Email, m => m.MapFrom(s => s.Email.Value))
            .ForMember(d => d.Phone, m => m.MapFrom(s => s.Phone == null ? null : s.Phone.Value))
            .ForMember(d => d.MoveInDate, m => m.MapFrom(s => s.MoveInDate))
            .ForMember(d => d.MoveOutDate, m => m.MapFrom(s => s.MoveOutDate))
            .ForMember(d => d.IsActive, m => m.MapFrom(s => s.Status == TenantStatus.Active))
            .ForMember(d => d.Apartment, m => m.Ignore());

        CreateMap<Apartment, TenantApartment>()
            .ForMember(d => d.Id, m => m.MapFrom(s => s.Id.Value.ToString()))
            .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
            .ForMember(d => d.UnitNumber, m => m.MapFrom(s=> s.UnitNumber));
    }
}
