using AutoMapper;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.DataContracts.User;
using Vokabular.MainService.DataContracts.Contracts;
using UserContactContract = Vokabular.MainService.DataContracts.Contracts.UserContactContract;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class UserContactProfile : Profile
    {
        public UserContactProfile()
        {
            CreateMap<UserContactContract, ContactContract>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ContactType, opt => opt.MapFrom(src => src.ContactType))
                .ForMember(dest => dest.ContactValue, opt => opt.Ignore());

            CreateMap<ConfirmUserContactContract, ConfirmContactContract>()
                .IncludeBase<UserContactContract, ContactContract>()
                .ForMember(dest => dest.ConfirmCode, opt => opt.MapFrom(src => src.ConfirmCode));

            CreateMap<UpdateUserContactContract, ChangeContactContract>()
                .IncludeBase<UserContactContract, ContactContract>()
                .ForMember(dest => dest.NewContactValue, opt => opt.MapFrom(src => src.NewContactValue));

            CreateMap<UpdateTwoFactorContract, ChangeTwoFactorContract>()
                .ForMember(dest => dest.TwoFactorIsEnabled, opt => opt.MapFrom(src => src.TwoFactorIsEnabled))
                .ForMember(dest => dest.TwoFactorProvider, opt => opt.MapFrom(src => src.TwoFactorProvider));
        }
    }
}
