using System.Collections.Generic;
using AutoMapper;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class WordListCriteriaDescriptionProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<WordListCriteriaDescription, WordListCriteriaContract>()
                .ForMember(dest => dest.Values, opt => opt.MapFrom(src => Mapper.Map<IList<WordCriteriaContract>>(src.WordCriteriaDescription)))
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => (CriteriaKey) src.SearchType))
                .ForAllMembers(opt => opt.Condition(src => !src.IsSourceValueNull));
        }
    }
}