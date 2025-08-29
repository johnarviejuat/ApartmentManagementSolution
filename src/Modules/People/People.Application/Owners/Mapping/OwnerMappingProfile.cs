using AutoMapper;
using People.Application.Common;
using People.Domain.Entities;

namespace People.Application.Owners.Mapping;
public class OwnerMappingProfile : Profile
{
    public OwnerMappingProfile()
    {
        // Mapping for OwnerId to Guid (explicit mapping)
        CreateMap<OwnerId, Guid>().ConvertUsing(id => id.Value);

        // Mapping for OwnerDto (mapping from Owner domain entity)
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
    }
}
