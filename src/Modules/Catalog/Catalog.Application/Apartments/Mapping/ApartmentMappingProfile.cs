using AutoMapper;
using Catalog.Application.Common;
using Catalog.Domain.Entities;
using People.Domain.Entities;

namespace Catalog.Application.Apartments.Mapping;
public sealed class ApartmentMappingProfile : Profile
{
    public ApartmentMappingProfile()
    {
        CreateMap<ApartmentId, Guid>().ConvertUsing(id => id.Value);
        CreateMap<Guid, ApartmentId>().ConvertUsing(g => new ApartmentId(g));

        CreateMap<Address, AddressDto>();

        CreateMap<Apartment, ApartmentDto>()
            .ForMember(d => d.Id, m => m.MapFrom(s => s.Id)) // uses converter
            .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
            .ForMember(d => d.UnitNumber, m => m.MapFrom(s => s.UnitNumber))
            .ForMember(d => d.Address, m => m.MapFrom(s => s.Address))
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
            .ForMember(d => d.CreatedAt, m => m.MapFrom(s => s.CreatedAt))
            .ForMember(d => d.UpdatedAt, m => m.MapFrom(s => s.UpdatedAt))
            .ForMember(d => d.Status, m => m.MapFrom(s => (int)s.Status));

        CreateMap<Owner, OwnerDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name.First))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name.Last))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone.Value))
            .ForMember(dest => dest.MailingLine1, opt => opt.MapFrom(src => src.MailingAddress.Line1))
            .ForMember(dest => dest.MailingCity, opt => opt.MapFrom(src => src.MailingAddress.City))
            .ForMember(dest => dest.MailingState, opt => opt.MapFrom(src => src.MailingAddress.State))
            .ForMember(dest => dest.MailingPostalCode, opt => opt.MapFrom(src => src.MailingAddress.PostalCode))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        CreateMap<Tenant, TenantDto>()
            .ForMember(d => d.ApartmentId, m => m.MapFrom(s => s.ApartmentId));
    }
}
