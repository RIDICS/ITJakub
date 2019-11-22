using AutoMapper;
using Scalesoft.Localization.AspNetCore;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile(ILocalizationService localization)
        {
            CreateMap<UserContract, string>()
                .ConvertUsing(src => 
                    (src == null || (src.FirstName == null && src.LastName == null))
                        ? $"({localization.Translate("NotFilled")})"
                        : $"{src.FirstName} {src.LastName}");
        }
    }
}