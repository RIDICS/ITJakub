﻿using AutoMapper;
using ITJakub.Web.Hub.Models;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class UserDetailProfile : Profile
    {
        public UserDetailProfile()
        {
            CreateMap<UserDetailContract, UserDetailViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<UserContract, UserDetailViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<UserDetailContract, UpdateContactViewModel>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.IsEmailConfirmed, opt => opt.MapFrom(src => src.IsEmailConfirmed))
                .ForMember(dest => dest.ConfirmCode, opt => opt.Ignore());
        }
    }
}
