using AutoMapper;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles
{
    public class LiteraryKindProfile : Profile
    {
        public LiteraryKindProfile()
        {
            CreateMap<LiteraryKindContract, int>().ConvertUsing(src => src.Id);
        }
    }
}