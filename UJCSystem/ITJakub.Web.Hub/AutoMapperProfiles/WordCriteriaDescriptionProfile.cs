using AutoMapper;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class WordCriteriaDescriptionProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<WordCriteriaDescription, WordCriteriaContract>()
                .ForMember(dest => dest.StartsWith, opt => opt.MapFrom(src => src.StartsWith))
                .ForMember(dest => dest.Contains, opt => opt.MapFrom(src => src.Contains)) 
                .ForMember(dest => dest.EndsWith, opt => opt.MapFrom(src => src.EndsWith)) 
                .ForAllMembers(opt => opt.Condition(src => !src.IsSourceValueNull));
        }
    }
}