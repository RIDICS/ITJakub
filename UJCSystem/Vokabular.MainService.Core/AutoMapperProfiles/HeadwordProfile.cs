using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class HeadwordProfile : Profile
    {
        public HeadwordProfile()
        {
            CreateMap<HeadwordResource, HeadwordContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Resource.Id))
                .ForMember(dest => dest.VersionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.VersionNumber, opt => opt.MapFrom(src => src.VersionNumber))
                .ForMember(dest => dest.DefaultHeadword, opt => opt.MapFrom(src => src.DefaultHeadword))
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.ExternalId))
                .ForMember(dest => dest.Sorting, opt => opt.MapFrom(src => src.Sorting))
                .ForMember(dest => dest.HeadwordItems, opt => opt.MapFrom(src => src.HeadwordItems));

            CreateMap<HeadwordItem, HeadwordItemContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Headword, opt => opt.MapFrom(src => src.Headword))
                .ForMember(dest => dest.HeadwordOriginal, opt => opt.MapFrom(src => src.HeadwordOriginal))
                .ForMember(dest => dest.ResourcePageId, opt => opt.MapFrom(src => src.ResourcePage.Id));
        }
    }
}