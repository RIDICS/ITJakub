using System.Globalization;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<MetadataResource, BookContract>()
                .IncludeBase<MetadataResource, ProjectMetadataContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Resource.Project.Id));

            CreateMap<MetadataResource, BookWithCategoriesContract>()
                .IncludeBase<MetadataResource, BookContract>()
                .ForMember(dest => dest.CategoryList, opt => opt.MapFrom(src => src.Resource.Project.Categories));

            CreateMap<MetadataResource, SearchResultContract>()
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.Resource.Project.Id)) // TODO try convert property from BookId to ProjectId (including TypeScript)
                .ForMember(dest => dest.BookXmlId, opt => opt.MapFrom(src => src.Resource.Project.ExternalId))
                .ForMember(dest => dest.BookType, opt => opt.MapFrom(src => src.Resource.Project.LatestPublishedSnapshot.DefaultBookType.Type))
                .ForMember(dest => dest.AuthorsLabel, opt => opt.MapFrom(src => src.AuthorsLabel))
                //.ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Resource.Project.ExternalId)) // Missing fetch
                .ForMember(dest => dest.BiblText, opt => opt.MapFrom(src => src.BiblText))
                .ForMember(dest => dest.Copyright, opt => opt.MapFrom(src => src.Copyright))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.CreateTimeString, opt => opt.MapFrom(src => src.CreateTime.ToString(CultureInfo.GetCultureInfo("cs")))) // Czech is the main language
                .ForMember(dest => dest.ManuscriptCountry, opt => opt.MapFrom(src => src.ManuscriptCountry))
                .ForMember(dest => dest.ManuscriptTitle, opt => opt.MapFrom(src => src.ManuscriptTitle))
                .ForMember(dest => dest.ManuscriptExtent, opt => opt.MapFrom(src => src.ManuscriptExtent))
                .ForMember(dest => dest.ManuscriptIdno, opt => opt.MapFrom(src => src.ManuscriptIdno))
                .ForMember(dest => dest.ManuscriptRepository, opt => opt.MapFrom(src => src.ManuscriptRepository))
                .ForMember(dest => dest.ManuscriptSettlement, opt => opt.MapFrom(src => src.ManuscriptSettlement))
                .ForMember(dest => dest.NotAfter, opt => opt.MapFrom(src => src.NotAfter))
                .ForMember(dest => dest.NotBefore, opt => opt.MapFrom(src => src.NotBefore))
                .ForMember(dest => dest.OriginDate, opt => opt.MapFrom(src => src.OriginDate))
                //.ForMember(dest => dest.PageCount, opt => opt.MapFrom(src => src.ExternalId)) // Missing fetch
                .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => src.PublishDate))
                .ForMember(dest => dest.PublishPlace, opt => opt.MapFrom(src => src.PublishPlace))
                .ForMember(dest => dest.PublisherText, opt => opt.MapFrom(src => src.PublisherText))
                .ForMember(dest => dest.PublisherEmail, opt => opt.MapFrom(src => src.PublisherEmail))
                .ForMember(dest => dest.RelicAbbreviation, opt => opt.MapFrom(src => src.RelicAbbreviation))
                .ForMember(dest => dest.SourceAbbreviation, opt => opt.MapFrom(src => src.SourceAbbreviation))
                .ForMember(dest => dest.SubTitle, opt => opt.MapFrom(src => src.SubTitle))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));
                //.ForMember(dest => dest.TermsPageHitsCount, opt => opt.MapFrom(src => src.ExternalId)) // Missing fetch
                //.ForMember(dest => dest.TotalHitCount, opt => opt.MapFrom(src => src.ExternalId)) // Missing fetch
                //.ForMember(dest => dest.VersionXmlId, opt => opt.MapFrom(src => src.ExternalId)); // Missing fetch

            CreateMap<MetadataResource, SearchResultDetailContract>()
                .IncludeBase<MetadataResource, SearchResultContract>()
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Resource.Project.Authors))
                .ForMember(dest => dest.Editors, opt => opt.MapFrom(src => src.Resource.Project.ResponsiblePersons.Where(x => x.ResponsibleType.Type == ResponsibleTypeEnum.Editor)))
                .ForMember(dest => dest.Keywords, opt => opt.MapFrom(src => src.Resource.Project.Keywords))
                .ForMember(dest => dest.LiteraryGenres, opt => opt.MapFrom(src => src.Resource.Project.LiteraryGenres))
                .ForMember(dest => dest.LiteraryKinds, opt => opt.MapFrom(src => src.Resource.Project.LiteraryKinds))
                .ForMember(dest => dest.LiteraryOriginals, opt => opt.MapFrom(src => src.Resource.Project.LiteraryOriginals));

            CreateMap<MetadataResource, AudioBookSearchResultContract>()
                .IncludeBase<MetadataResource, SearchResultContract>();
        }
    }
}