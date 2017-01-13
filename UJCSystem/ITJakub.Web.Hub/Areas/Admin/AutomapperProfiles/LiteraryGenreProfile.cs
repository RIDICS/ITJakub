using AutoMapper;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles
{
    public class LiteraryGenreProfile : Profile
    {
        public LiteraryGenreProfile()
        {
            CreateMap<LiteraryGenreContract, int>().ConvertUsing(src => src.Id);
        }
    }
}