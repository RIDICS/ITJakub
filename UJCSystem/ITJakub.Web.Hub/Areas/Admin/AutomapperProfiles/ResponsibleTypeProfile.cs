using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.Models.Type;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles
{
    public class ResponsibleTypeProfile : Profile
    {
        public ResponsibleTypeProfile()
        {
            CreateMap<ResponsibleTypeContract, ResponsibleTypeViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));

            CreateMap<ResponsibleTypeEnumContract, ResponsibleTypeEnumViewModel>().ReverseMap();
        }
    }
}