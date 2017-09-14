using System.Collections.Generic;
using AutoMapper;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class DatingListCriteriaDescriptionProfile : Profile
    {
        public DatingListCriteriaDescriptionProfile()
        {
            CreateMap<DatingListCriteriaDescription, DatingListCriteriaContract>()
                .ForMember(dest => dest.Disjunctions, opt => opt.MapFrom(src => Mapper.Map<IList<DatingCriteriaContract>>(src.Disjunctions)))
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => (CriteriaKey) src.SearchType))
                .ForAllMembers(opt => opt.Condition(src => src != null));
        }
    }
}