using AutoMapper;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class TokenDistanceCriteriaDescriptionProfile : Profile
    {
        public TokenDistanceCriteriaDescriptionProfile()
        {
            CreateMap<TokenDistanceCriteriaDescription, TokenDistanceCriteriaContract>()
                .ForMember(dest => dest.Distance, opt => opt.MapFrom(src => src.Distance))
                .ForMember(dest => dest.First, opt => opt.MapFrom(src =>  Mapper.Map<WordCriteriaContract>(src.First))) 
                .ForMember(dest => dest.Second, opt => opt.MapFrom(src => Mapper.Map<WordCriteriaContract>(src.Second))) 
                .ForAllMembers(opt => opt.Condition(src => src != null));
        }
    }
}