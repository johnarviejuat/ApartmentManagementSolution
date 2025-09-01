using AutoMapper;
using Catalog.Application.Common;
using Catalog.Domain.Entities;
using People.Domain.Entities;

namespace Catalog.Application.Apartments.Mapping
{
    public sealed class ApartmentMappingProfile : Profile
    {
        public ApartmentMappingProfile()
        {
            CreateMap<ApartmentId, Guid>().ConvertUsing(id => id.Value);

            CreateMap<Apartment, ApartmentDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id.Value))
                .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
                .ForMember(d => d.UnitNumber, m => m.MapFrom(s => s.UnitNumber))
                .ForMember(d => d.Line1, m => m.MapFrom(s => s.Address.Line1))
                .ForMember(d => d.City, m => m.MapFrom(s => s.Address.City))
                .ForMember(d => d.State, m => m.MapFrom(s => s.Address.State))
                .ForMember(d => d.PostalCode, m => m.MapFrom(s => s.Address.PostalCode))
                .ForMember(d => d.Bedrooms, m => m.MapFrom(s => s.Bedrooms))
                .ForMember(d => d.Bathrooms, m => m.MapFrom(s => s.Bathrooms))
                .ForMember(d => d.Capacity, m => m.MapFrom(s => s.Capacity))
                .ForMember(d => d.CurrentCapacity, m => m.MapFrom(s => s.CurrentCapacity))
                .ForMember(d => d.SquareFeet, m => m.MapFrom(s => s.SquareFeet))
                .ForMember(d => d.MonthlyRent, m => m.MapFrom(s => s.MonthlyRent))
                .ForMember(d => d.AdvanceRent, m => m.MapFrom(s => s.AdvanceRent))
                .ForMember(d => d.SecurityDeposit, m => m.MapFrom(s => s.SecurityDeposit))
                .ForMember(d => d.AvailableFrom, m => m.MapFrom(s => s.AvailableFrom))
                .ForMember(d => d.IsAvailable, m => m.MapFrom(s => s.IsAvailable))
                .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                .ForMember(d => d.Status, m => m.MapFrom(s => (int)s.Status));

            CreateMap<Owner, PersonInformation>() 
              .ForMember(d => d.Id, m => m.MapFrom(s => s.Id.Value.ToString()))
              .ForMember(d => d.FirstName, m => m.MapFrom(s => s.Name.First))
              .ForMember(d => d.LastName, m => m.MapFrom(s => s.Name.Last))
              .ForMember(d => d.Email, m => m.MapFrom(s => s.Email.Value))
              .ForMember(d => d.Phone, m => m.MapFrom(s => s.Phone == null ? null : s.Phone.Value));

            CreateMap<Tenant, PersonInformation>()
              .ForMember(d => d.Id, m => m.MapFrom(s => s.Id.Value.ToString()))
              .ForMember(d => d.FirstName, m => m.MapFrom(s => s.Name.First))
              .ForMember(d => d.LastName, m => m.MapFrom(s => s.Name.Last))
              .ForMember(d => d.Email, m => m.MapFrom(s => s.Email.Value))
              .ForMember(d => d.Phone, m => m.MapFrom(s => s.Phone == null ? null : s.Phone.Value));
        }
    }
}
