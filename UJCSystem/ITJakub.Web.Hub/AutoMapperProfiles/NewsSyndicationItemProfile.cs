using AutoMapper;
using ITJakub.Web.Hub.DataContracts;
using Scalesoft.Localization.AspNetCore;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class NewsSyndicationItemProfile : Profile
    {
        public NewsSyndicationItemProfile(ILocalizationService localizationService)
        {
            CreateMap<NewsSyndicationItemContract, NewsSyndicationItemExtendedContract>()
                .ForMember(dest => dest.CreateTimeString, opt => opt.MapFrom(src => src.CreateTime.ToLocalTime().ToString(localizationService.GetRequestCulture())));
        }
    }
}