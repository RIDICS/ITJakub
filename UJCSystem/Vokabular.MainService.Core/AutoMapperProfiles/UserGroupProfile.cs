using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class UserGroupProfile : Profile
    {
        public UserGroupProfile()
        {
            CreateMap<SingleUserGroup, RoleContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ExternalId, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => $"{src.User.ExtFirstName} {src.User.ExtLastName} - {src.User.ExtUsername}"));

            CreateMap<SingleUserGroup, UserGroupContract>()
                .IncludeBase<SingleUserGroup, RoleContract>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => UserGroupTypeContract.Single));

            CreateMap<SingleUserGroup, RoleDetailContract>()
                .IncludeBase<SingleUserGroup, UserGroupContract>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => new List<SpecialPermissionContract>()));


            CreateMap<RoleUserGroup, UserGroupContract>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => UserGroupTypeContract.Role))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.ExternalId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.Ignore());
        }
    }
}