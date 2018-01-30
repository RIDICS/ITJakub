using AutoMapper;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class WordCriteriaDescriptionProfile : Profile
    {
        public WordCriteriaDescriptionProfile()
        {
            CreateMap<WordCriteriaDescription, WordCriteriaContract>()
                .ForMember(dest => dest.StartsWith, opt => opt.MapFrom(src => src.StartsWith))
                .ForMember(dest => dest.Contains, opt => opt.MapFrom(src => src.Contains)) 
                .ForMember(dest => dest.EndsWith, opt => opt.MapFrom(src => src.EndsWith)) 
                .ForMember(dest => dest.ExactMatch, opt => opt.MapFrom(src => src.ExactMatch)) 
                .ForAllMembers(opt => opt.Condition(src => src != null));
        }
    }
}