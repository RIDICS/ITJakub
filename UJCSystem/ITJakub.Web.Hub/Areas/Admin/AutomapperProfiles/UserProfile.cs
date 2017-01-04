using AutoMapper;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserContract, string>()
                .ConvertUsing(src => 
                    (src == null || (src.FirstName == null && src.LastName == null))
                        ? "(nezadáno)"
                        : $"{src.FirstName} {src.LastName}");
        }
    }
}