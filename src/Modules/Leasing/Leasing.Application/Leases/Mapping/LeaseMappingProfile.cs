using AutoMapper;
using Catalog.Domain.Entities;
using Leasing.Application.Common;
using Leasing.Domain.Entities;
using People.Domain.Entities;

namespace Leasing.Application.Leases.Mapping
{
    public class LeaseMappingProfile : Profile
    {
        public LeaseMappingProfile()
        {
            CreateMap<Lease, LeaseDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value))
                .ForMember(d => d.MonthlyRent, opt => opt.MapFrom(s => s.MonthlyRent))
                .ForMember(d => d.NextDueDate, opt => opt.MapFrom(s => s.NextDueDate))
                .ForMember(d => d.Credit, opt => opt.MapFrom(s => s.Credit))
                .ForMember(d => d.DepositHeld, opt => opt.MapFrom(s => s.DepositHeld))
                .ForMember(d => d.IsDepositFunded, opt => opt.MapFrom(s => s.IsDepositFunded))
                .ForMember(d => d.IsActive, opt => opt.MapFrom(s => s.IsActive));

            CreateMap<Apartment, ApartmentInformation>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.UnitNumber, opt => opt.MapFrom(s => s.UnitNumber))
                .ForMember(d => d.Line1, opt => opt.MapFrom(s => s.Address.Line1))
                .ForMember(d => d.City, opt => opt.MapFrom(s => s.Address.City))
                .ForMember(d => d.State, opt => opt.MapFrom(s => s.Address.State))
                .ForMember(d => d.PostalCode, opt => opt.MapFrom(s => s.Address.PostalCode))
                .ForMember(d => d.Capacity, opt => opt.MapFrom(s => s.Capacity));

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
