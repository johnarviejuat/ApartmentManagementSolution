using AutoMapper;
using ApartmentManagement.Application.Common;
using ApartmentManagement.Domain.Leasing.Apartments;

namespace ApartmentManagement.Application.Apartments.Mapping
{
    public class ApartmentMappingProfile : Profile
    {
        public ApartmentMappingProfile()
        {
            CreateMap<ApartmentId, Guid>().ConvertUsing(id => id.Value);

            CreateMap<Apartment, ApartmentDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UnitNumber, opt => opt.MapFrom(src => src.UnitNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Bedrooms, opt => opt.MapFrom(src => src.Bedrooms))
                .ForMember(dest => dest.Bathrooms, opt => opt.MapFrom(src => src.Bathrooms))
                .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Capacity))
                .ForMember(dest => dest.CurrentCapacity, opt => opt.MapFrom(src => src.CurrentCapacity))
                .ForMember(dest => dest.SquareFeet, opt => opt.MapFrom(src => src.SquareFeet))
                .ForMember(dest => dest.MonthlyRent, opt => opt.MapFrom(src => src.MonthlyRent))
                .ForMember(dest => dest.MonthlyRent, opt => opt.MapFrom(src => src.MonthlyRent))
                .ForMember(dest => dest.AdvanceRent, opt => opt.MapFrom(src => src.MonthlyRent))
                .ForMember(dest => dest.SecurityDeposit, opt => opt.MapFrom(src => src.MonthlyRent))
                .ForMember(dest => dest.AvailableFrom, opt => opt.MapFrom(src => src.AvailableFrom))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

            CreateMap<Address, AddressDto>();
        }
    }
}
