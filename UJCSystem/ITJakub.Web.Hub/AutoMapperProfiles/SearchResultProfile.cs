using AutoMapper;
using ITJakub.Web.Hub.DataContracts;
using ITJakub.Web.Hub.Helpers;
using Scalesoft.Localization.AspNetCore;
using Vokabular.MainService.DataContracts.Contracts.Search;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class SearchResultProfile : Profile
    {
        public SearchResultProfile(ILocalizationService localizationService, ProjectTypeLocalizer projectTypeLocalizer)
        {
            CreateMap<SearchResultContract, SearchResultExtendedContract>()
                .ForMember(dest => dest.ProjectTypeString, opt => opt.MapFrom(src => projectTypeLocalizer.TranslateProjectType(src.ProjectType)))
                .ForMember(dest => dest.CreateTimeString, opt => opt.MapFrom(src => src.CreateTime.ToLocalTime().ToString(localizationService.GetRequestCulture())));

            CreateMap<SearchResultDetailContract, SearchResultDetailExtendedContract>()
                .ForMember(dest => dest.ProjectTypeString, opt => opt.MapFrom(src => projectTypeLocalizer.TranslateProjectType(src.ProjectType)))
                .ForMember(dest => dest.CreateTimeString, opt => opt.MapFrom(src => src.CreateTime.ToLocalTime().ToString(localizationService.GetRequestCulture())));

            CreateMap<AudioBookSearchResultContract, AudioBookSearchResultExtendedContract>()
                .ForMember(dest => dest.ProjectTypeString, opt => opt.MapFrom(src => projectTypeLocalizer.TranslateProjectType(src.ProjectType)))
                .ForMember(dest => dest.CreateTimeString, opt => opt.MapFrom(src => src.CreateTime.ToLocalTime().ToString(localizationService.GetRequestCulture())));
        }
    }
}