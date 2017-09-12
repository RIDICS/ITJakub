using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class MetadataProfile : Profile
    {
        public MetadataProfile()
        {
            CreateMap<MetadataResource, ProjectMetadataContract>()
                .ForMember(dest => dest.LastModification, opt => opt.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.SubTitle, opt => opt.MapFrom(src => src.SubTitle))
                .ForMember(dest => dest.RelicAbbreviation, opt => opt.MapFrom(src => src.RelicAbbreviation))
                .ForMember(dest => dest.SourceAbbreviation, opt => opt.MapFrom(src => src.SourceAbbreviation))
                .ForMember(dest => dest.PublishPlace, opt => opt.MapFrom(src => src.PublishPlace))
                .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => src.PublishDate))
                .ForMember(dest => dest.PublisherText, opt => opt.MapFrom(src => src.PublisherText))
                .ForMember(dest => dest.PublisherEmail, opt => opt.MapFrom(src => src.PublisherEmail))
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
                .ForMember(dest => dest.ManuscriptTitle, opt => opt.MapFrom(src => src.ManuscriptTitle))
                .Include<MetadataResource, ProjectMetadataResultContract>();

            CreateMap<MetadataResource, ProjectMetadataResultContract>();
        }
    }
}