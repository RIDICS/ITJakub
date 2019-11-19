﻿using AutoMapper;
using Ridics.Core.Shared.Types;
using Vokabular.MainService.DataContracts.Contracts;
using ContactTypeEnum = Ridics.Authentication.DataContracts.ContactTypeEnum;

namespace Vokabular.MainService.Core.AutoMapperProfiles.Authentication
{
    public class AuthUserProfile : Profile
    {
        public AuthUserProfile()
        {
            CreateMap<Ridics.Authentication.DataContracts.User.UserContract, UserContract>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Ridics.Authentication.DataContracts.User.UserContract, UserDetailContract>()
                .IncludeBase<Ridics.Authentication.DataContracts.User.UserContract, UserContract>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.IsEmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
                .ForMember(dest => dest.TwoFactorProvider, opt => opt.MapFrom(src => src.TwoFactorProvider))
                .ForMember(dest => dest.ValidTwoFactorProviders, opt => opt.MapFrom(src => src.ValidTwoFactorProviders));

            CreateMap<Ridics.Authentication.DataContracts.UserWithRolesContract, UserContract>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.Ignore()) // Username is missing in source data
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<Ridics.Authentication.DataContracts.BasicUserInfoContract, UserContract>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.UserData[UserDataTypes.FirstName]))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.UserData[UserDataTypes.LastName]))
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Ridics.Authentication.DataContracts.BasicUserInfoContract, UserWithContactContract>()
                .IncludeBase<Ridics.Authentication.DataContracts.BasicUserInfoContract, UserContract>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserContact[ContactTypeEnum.Email.ToString()]));
        }
    }
}
