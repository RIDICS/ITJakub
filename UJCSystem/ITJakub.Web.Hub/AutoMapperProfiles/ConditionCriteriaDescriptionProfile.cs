using AutoMapper;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts.Searching;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class ConditionCriteriaDescriptionProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<ConditionCriteriaDescription, SearchCriteriaContract>()
                .Include<DatingListCriteriaDescription, DatingListCriteriaContract>()
                .Include<WordListCriteriaDescription, WordListCriteriaContract>();
        }
    }
}