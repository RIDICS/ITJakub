using AutoMapper;
using ITJakub.Web.Hub.Models;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<PermissionContract, PermissionViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.IsSelected, opt => opt.Ignore());
        }
    }
}
