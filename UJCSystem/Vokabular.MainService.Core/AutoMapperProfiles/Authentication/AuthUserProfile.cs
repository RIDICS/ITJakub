using AutoMapper;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles.Authentication
{
    public class AuthUserProfile : Profile
    {
        public AuthUserProfile()
        {
            CreateMap<Vokabular.Authentication.DataContracts.User.UserContract, UserContract>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Vokabular.Authentication.DataContracts.User.UserContract, UserDetailContract>()
                .IncludeBase<Vokabular.Authentication.DataContracts.User.UserContract, UserContract>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));


            CreateMap<Vokabular.Authentication.DataContracts.UserWithRolesContract, UserContract>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.Ignore()) // Username is missing in source data
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
