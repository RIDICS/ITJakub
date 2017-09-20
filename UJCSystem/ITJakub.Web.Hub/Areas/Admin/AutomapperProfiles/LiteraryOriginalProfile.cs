using AutoMapper;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles
{
    public class LiteraryOriginalProfile : Profile
    {
        public LiteraryOriginalProfile()
        {
            CreateMap<LiteraryOriginalContract, int>().ConvertUsing(src => src.Id);
        }
    }
}