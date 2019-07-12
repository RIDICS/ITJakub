using AutoMapper;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Core.AutoMapperProfiles.Authentication
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Ridics.Authentication.DataContracts.RoleContract, RoleContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<Ridics.Authentication.DataContracts.RoleContract, RoleDetailContract>()
                .IncludeBase<Ridics.Authentication.DataContracts.RoleContract, RoleContract>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));
        }
    }
}
