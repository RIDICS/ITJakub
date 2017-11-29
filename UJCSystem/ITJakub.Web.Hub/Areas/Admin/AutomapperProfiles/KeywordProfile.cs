using AutoMapper;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles
{
    public class KeywordProfile : Profile
    {
        public KeywordProfile()
        {
            CreateMap<KeywordContract, int>().ConvertUsing(src => src.Id);
        }
    }
}
