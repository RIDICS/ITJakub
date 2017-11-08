using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class TextProfile : Profile
    {
        public TextProfile()
        {
            CreateMap<TextResource, TextContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Resource.Id))
                .ForMember(dest => dest.VersionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.VersionNumber, opt => opt.MapFrom(src => src.VersionNumber))
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.ExternalId))
                .ForMember(dest => dest.BookVersionId, opt => opt.MapFrom(src => src.BookVersion.Id));

            CreateMap<TextResource, FullTextContract>()
                .IncludeBase<TextResource, TextContract>();

            CreateMap<TextResource, TextWithPageContract>()
                .IncludeBase<TextResource, TextContract>()
                .ForMember(dest => dest.ParentPage, opt => opt.MapFrom(src => src.ResourcePage.LatestVersion));
        }
    }
}