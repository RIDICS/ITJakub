using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class ResourceVersionProfile : Profile
    {
        public ResourceVersionProfile()
        {
            CreateMap<ResourceVersion, ResourceVersionContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.VersionNumber, opt => opt.MapFrom(src => src.VersionNumber))
                .ForMember(dest => dest.Author, opt => opt.Ignore())
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateTime));
        }
    }
}