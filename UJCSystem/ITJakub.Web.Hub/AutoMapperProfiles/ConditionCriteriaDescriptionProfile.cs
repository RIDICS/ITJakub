using AutoMapper;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Vokabular.Shared.DataContracts.Search.Criteria;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class ConditionCriteriaDescriptionProfile : Profile
    {

        public ConditionCriteriaDescriptionProfile()
        {
            CreateMap<ConditionCriteriaDescriptionBase, SearchCriteriaContract>()
                .Include<DatingListCriteriaDescription, DatingListCriteriaContract>()
                .Include<WordListCriteriaDescription, WordListCriteriaContract>()
                .Include<TokenDistanceListCriteriaDescription, TokenDistanceListCriteriaContract>();
        }
    }
}
