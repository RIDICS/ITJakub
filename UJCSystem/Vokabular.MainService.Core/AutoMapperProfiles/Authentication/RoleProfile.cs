﻿using AutoMapper;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Core.AutoMapperProfiles.Authentication
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Vokabular.Authentication.DataContracts.RoleContract, UserGroupContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<Vokabular.Authentication.DataContracts.RoleContract, UserGroupDetailContract>()
                .IncludeBase<Vokabular.Authentication.DataContracts.RoleContract, UserGroupContract>()
                .ForMember(dest => dest.Members, opt => opt.Ignore());
        }
    }
}
