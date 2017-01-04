using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.Models;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<ProjectContract, ProjectItemViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate.ToLocalTime()))
                .ForMember(dest => dest.CreateUser, opt => opt.MapFrom(src => src.CreateUser))
                .ForMember(dest => dest.LastEditDate, opt => opt.MapFrom(src => src.LastEditDate.ToLocalTime()))
                .ForMember(dest => dest.LastEditUser, opt => opt.MapFrom(src => src.LastEditUser))
                .ForMember(dest => dest.LiteraryOriginalText, opt => opt.MapFrom(src => src.LiteraryOriginalText))
                .ForMember(dest => dest.PageCount, opt => opt.MapFrom(src => src.PageCount))
                .ForMember(dest => dest.PublisherText, opt => opt.MapFrom(src => src.PublisherText));

            CreateMap<ProjectMetadataContract, ProjectWorkMetadataViewModel>()
                .ForMember(dest => dest.Editor, opt => opt.MapFrom(src => src.Editor))
                .ForMember(dest => dest.LastModification, opt => opt.MapFrom(src => src.LastModification.ToLocalTime()))
                .ForMember(dest => dest.LiteraryGenre, opt => opt.MapFrom(src => src.LiteraryGenre))
                .ForMember(dest => dest.LiteraryKind, opt => opt.MapFrom(src => src.LiteraryKind))
                .ForMember(dest => dest.RelicAbbreviation, opt => opt.MapFrom(src => src.RelicAbbreviation))
                .ForMember(dest => dest.SourceAbbreviation, opt => opt.MapFrom(src => src.SourceAbbreviation))
                .ForMember(dest => dest.LiteraryOriginal, opt => opt.MapFrom(src => src.LiteraryOriginal));

            CreateMap<ProjectLiteraryOriginalContract, ProjectWorkLiteraryOriginalViewModel>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.Extent, opt => opt.MapFrom(src => src.Extent))
                .ForMember(dest => dest.Institution, opt => opt.MapFrom(src => src.Institution))
                .ForMember(dest => dest.Signature, opt => opt.MapFrom(src => src.Signature));
        }
    }
}