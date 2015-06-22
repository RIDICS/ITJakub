using System.Collections.Generic;
using AutoMapper;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class ConditionCriteriaDescriptionProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<ConditionCriteriaDescription, WordListCriteriaContract>()
                .ForMember(dest => dest.Values, opt => opt.MapFrom(src => Mapper.Map<IList<WordCriteriaContract>>(src.WordCriteriaDescription)))
                //.ForMember(dest => dest.Key, opt => opt.MapFrom(src => Enum.GetName(typeof(CriteriaKey), src.SearchType))) //TODO string to enum
                .ForAllMembers(opt => opt.Condition(src => !src.IsSourceValueNull));
        }
    }
}