using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class UserGroupProfile : Profile
    {
        public UserGroupProfile()
        {
            CreateMap<SingleUserGroup, RoleContract>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.Ignore());

            CreateMap<SingleUserGroup, RoleDetailContract>()
                .IncludeBase<SingleUserGroup, RoleContract>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => new List<PermissionContract>()));
        }
    }
}