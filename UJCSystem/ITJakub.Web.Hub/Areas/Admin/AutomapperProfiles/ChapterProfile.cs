using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Areas.Admin.Models.Type;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles
{
    public class ChapterProfile : Profile
    {
        public ChapterProfile()
        {
            CreateMap<ChapterHierarchyDetailContract, ChapterHierarchyViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.BeginningPageId, opt => opt.MapFrom(src => src.BeginningPageId))
                .ForMember(dest => dest.BeginningPageName, opt => opt.MapFrom(src => src.BeginningPageName))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.SubChapters, opt => opt.MapFrom(src => src.SubChapters));
        }
    }
}