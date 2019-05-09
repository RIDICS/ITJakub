using System;
using AutoMapper;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class DatingCriteriaDescriptionProfile : Profile
    {
        public DatingCriteriaDescriptionProfile()
        {
            CreateMap<DatingCriteriaDescription, DatingCriteriaContract>()
                .ForMember(dest => dest.NotAfter, opt => opt.MapFrom(src => src.NotAfter.HasValue ? new DateTime?(new DateTime(src.NotAfter.Value,1,1)) : null))
                .ForMember(dest => dest.NotBefore, opt => opt.MapFrom(src => src.NotBefore.HasValue ? new DateTime?(new DateTime(src.NotBefore.Value,1,1)) : null))
                .ForAllMembers(opt => opt.Condition(src => src != null));
        }
    }
}