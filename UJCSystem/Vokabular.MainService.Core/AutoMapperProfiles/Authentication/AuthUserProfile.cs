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
                .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore())
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Vokabular.Authentication.DataContracts.User.UserContract, UserDetailContract>()
                .IncludeBase<Vokabular.Authentication.DataContracts.User.UserContract, UserContract>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        }
    }
}
