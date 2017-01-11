using System;
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
                .ForMember(dest => dest.LastModification, opt => opt.MapFrom(src => src.LastModification != null ? (DateTime?)src.LastModification.Value.ToLocalTime() : null))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Subtitle, opt => opt.MapFrom(src => src.SubTitle))
                .ForMember(dest => dest.RelicAbbreviation, opt => opt.MapFrom(src => src.RelicAbbreviation))
                .ForMember(dest => dest.SourceAbbreviation, opt => opt.MapFrom(src => src.SourceAbbreviation))
                .ForMember(dest => dest.PublisherId, opt => opt.MapFrom(src => src.PublisherId))
                .ForMember(dest => dest.PublishPlace, opt => opt.MapFrom(src => src.PublishPlace))
                .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => src.PublishDate))
                .ForMember(dest => dest.Copyright, opt => opt.MapFrom(src => src.Copyright))
                .ForMember(dest => dest.BiblText, opt => opt.MapFrom(src => src.BiblText))
                .ForMember(dest => dest.OriginDate, opt => opt.MapFrom(src => src.OriginDate))
                .ForMember(dest => dest.NotBefore, opt => opt.MapFrom(src => src.NotBefore))
                .ForMember(dest => dest.NotAfter, opt => opt.MapFrom(src => src.NotAfter))
                .ForMember(dest => dest.ManuscriptSettlement, opt => opt.MapFrom(src => src.ManuscriptSettlement))
                .ForMember(dest => dest.ManuscriptCountry, opt => opt.MapFrom(src => src.ManuscriptCountry))
                .ForMember(dest => dest.ManuscriptExtent, opt => opt.MapFrom(src => src.ManuscriptExtent))
                .ForMember(dest => dest.ManuscriptRepository, opt => opt.MapFrom(src => src.ManuscriptRepository))
                .ForMember(dest => dest.ManuscriptIdno, opt => opt.MapFrom(src => src.ManuscriptIdno))
                .Include<ProjectMetadataResultContract, ProjectWorkMetadataViewModel>();

            CreateMap<ProjectMetadataResultContract, ProjectWorkMetadataViewModel>();
        }
    }
}