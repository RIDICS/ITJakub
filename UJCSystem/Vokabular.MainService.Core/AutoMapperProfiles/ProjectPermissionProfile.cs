using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class ProjectPermissionProfile : Profile
    {
        public ProjectPermissionProfile()
        {
            CreateMap<Permission, PermissionDataContract>()
                .ForMember(dest => dest.ShowPublished, opt => opt.MapFrom(src => src.Flags.HasFlag(PermissionFlag.ShowPublished)))
                .ForMember(dest => dest.ReadProject, opt => opt.MapFrom(src => src.Flags.HasFlag(PermissionFlag.ReadProject)))
                .ForMember(dest => dest.EditProject, opt => opt.MapFrom(src => src.Flags.HasFlag(PermissionFlag.EditProject)))
                .ForMember(dest => dest.AdminProject, opt => opt.MapFrom(src => src.Flags.HasFlag(PermissionFlag.AdminProject)));
        }
    }
}