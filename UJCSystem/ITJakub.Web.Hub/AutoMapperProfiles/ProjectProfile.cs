using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.Models;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<ProjectContract, ProjectItemViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.CreateUser, opt => opt.MapFrom(src => src.CreateUser))
                .ForMember(dest => dest.LastEditDate, opt => opt.MapFrom(src => src.LastEditDate))
                .ForMember(dest => dest.LastEditUser, opt => opt.MapFrom(src => src.LastEditUser))
                .ForMember(dest => dest.LiteraryOriginalText, opt => opt.MapFrom(src => src.LiteraryOriginalText))
                .ForMember(dest => dest.PageCount, opt => opt.MapFrom(src => src.PageCount))
                .ForMember(dest => dest.PublisherText, opt => opt.MapFrom(src => src.PublisherText));
        }
    }
}