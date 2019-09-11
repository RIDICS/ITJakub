using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class ResourceProfile : Profile
    {
        public ResourceProfile()
        {
            CreateMap<Resource, ResourceContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ResourceType, opt => opt.MapFrom(src => src.ResourceType));

            CreateMap<Resource, ResourceWithLatestVersionContract>()
                .IncludeBase<Resource, ResourceContract>()
                .ForMember(dest => dest.LatestVersion, opt => opt.MapFrom(src => src.LatestVersion));

            CreateMap<ResourceTypeEnum, ResourceTypeEnumContract>().ReverseMap();
        }
    }
}