using System.Collections.Generic;
using AutoMapper;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class WordListCriteriaDescriptionProfile : Profile
    {
        public WordListCriteriaDescriptionProfile()
        {
            CreateMap<WordListCriteriaDescription, WordListCriteriaContract>()
                .ForMember(dest => dest.Disjunctions, opt => opt.MapFrom(src => Mapper.Map<IList<WordCriteriaContract>>(src.Disjunctions)))
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => (CriteriaKey)src.SearchType))
                .ForAllMembers(opt => opt.Condition(src => src != null));
        }
    }
}