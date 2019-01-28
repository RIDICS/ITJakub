using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class UserGroupProfile : Profile
    {
        public UserGroupProfile()
        {
            CreateMap<UserGroup, RoleContract>()
                .ForMember(m => m.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(m => m.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(m => m.Description, opt => opt.MapFrom(src => src.Description));
        }
    }
}