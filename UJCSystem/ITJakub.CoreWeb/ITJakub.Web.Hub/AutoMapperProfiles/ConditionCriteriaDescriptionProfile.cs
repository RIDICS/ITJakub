using AutoMapper;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class ConditionCriteriaDescriptionProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<ConditionCriteriaDescriptionBase, SearchCriteriaContract>()
                .Include<DatingListCriteriaDescription, DatingListCriteriaContract>()
                .Include<WordListCriteriaDescription, WordListCriteriaContract>()
                .Include<TokenDistanceListCriteriaDescription, TokenDistanceListCriteriaContract>();
        }
    }
}
