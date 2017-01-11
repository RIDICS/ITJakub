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
                .ForMember(dest => dest.LastModification, opt => opt.MapFrom(src => src.LastModification.ToLocalTime()))
                .ForMember(dest => dest.RelicAbbreviation, opt => opt.MapFrom(src => src.RelicAbbreviation))
                .ForMember(dest => dest.SourceAbbreviation, opt => opt.MapFrom(src => src.SourceAbbreviation))
                .ForMember(dest => dest.ManuscriptSettlement, opt => opt.MapFrom(src => src.ManuscriptSettlement))
                .ForMember(dest => dest.ManuscriptCountry, opt => opt.MapFrom(src => src.ManuscriptCountry))
                .ForMember(dest => dest.ManuscriptExtent, opt => opt.MapFrom(src => src.ManuscriptExtent))
                .ForMember(dest => dest.ManuscriptRepository, opt => opt.MapFrom(src => src.ManuscriptRepository))
                .ForMember(dest => dest.ManuscriptIdno, opt => opt.MapFrom(src => src.ManuscriptIdno));
        }
    }
}