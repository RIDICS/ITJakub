using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.Models.Type;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles
{
    public class ResponsibleTypeProfile : Profile
    {
        public ResponsibleTypeProfile()
        {
            //CreateMap<ResponsibleTypeContract>()

            CreateMap<ResponsibleTypeEnumContract, ResponsibleTypeEnumViewModel>().ReverseMap();
        }
    }
}