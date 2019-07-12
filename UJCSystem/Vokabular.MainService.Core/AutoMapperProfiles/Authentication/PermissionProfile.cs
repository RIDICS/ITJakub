using AutoMapper;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Core.AutoMapperProfiles.Authentication
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<Ridics.Authentication.DataContracts.PermissionContract, PermissionContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
        }
    }
}
