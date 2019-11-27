using AutoMapper;
using ITJakub.Web.Hub.DataContracts;
using Scalesoft.Localization.AspNetCore;
using Vokabular.MainService.DataContracts.Contracts.Feedback;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class FeedbackProfile : Profile
    {
        public FeedbackProfile(ILocalizationService localizationService)
        {
            CreateMap<FeedbackContract, FeedbackExtendedContract>()
                .ForMember(dest => dest.CreateTimeString, opt => opt.MapFrom(src => src.CreateTime.ToLocalTime().ToString(localizationService.GetRequestCulture())));
        }
    }
}