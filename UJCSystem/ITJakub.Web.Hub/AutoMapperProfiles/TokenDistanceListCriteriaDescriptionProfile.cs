using AutoMapper;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class TokenDistanceListCriteriaDescriptionProfile : Profile
    {
        public TokenDistanceListCriteriaDescriptionProfile()
        {
            CreateMap<TokenDistanceListCriteriaDescription, TokenDistanceListCriteriaContract>()
                .ForMember(dest => dest.Disjunctions, opt => opt.MapFrom(src => src.Disjunctions))
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => (CriteriaKey) src.SearchType))
                .ForAllMembers(opt => opt.Condition(src => src != null));
        }
    }
}