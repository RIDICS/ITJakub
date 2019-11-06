using AutoMapper;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.AutoMapperProfiles.Authentication
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Ridics.Authentication.DataContracts.RoleContractBase, RoleContract>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<Ridics.Authentication.DataContracts.RoleContractBase, UserGroupContract>()
                .IncludeBase<Ridics.Authentication.DataContracts.RoleContractBase, RoleContract>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => UserGroupTypeContract.Role));


            CreateMap<Ridics.Authentication.DataContracts.RoleContract, RoleContract>()
                .IncludeBase<Ridics.Authentication.DataContracts.RoleContractBase, RoleContract>();

            CreateMap<Ridics.Authentication.DataContracts.RoleContract, UserGroupContract>()
                .IncludeBase<Ridics.Authentication.DataContracts.RoleContractBase, RoleContract>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => UserGroupTypeContract.Role));


            CreateMap<Ridics.Authentication.DataContracts.RoleContract, RoleDetailContract>()
                .IncludeBase<Ridics.Authentication.DataContracts.RoleContract, RoleContract>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => UserGroupTypeContract.Role))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));
        }
    }
}
