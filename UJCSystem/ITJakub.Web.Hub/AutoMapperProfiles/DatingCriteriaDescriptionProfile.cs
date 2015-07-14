using AutoMapper;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class DatingCriteriaDescriptionProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<DatingCriteriaDescription, DatingCriteriaContract>()
                .ForMember(dest => dest.NotAfter, opt => opt.MapFrom(src => src.NotAfter))
                .ForMember(dest => dest.NotBefore, opt => opt.MapFrom(src => src.NotBefore)) 
                .ForAllMembers(opt => opt.Condition(src => !src.IsSourceValueNull));
        }
    }
}