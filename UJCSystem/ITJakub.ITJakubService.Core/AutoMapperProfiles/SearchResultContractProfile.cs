using System.Globalization;
using System.Linq;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.Shared.Contracts.Searching.Results;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class SearchResultContractProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<BookVersion, SearchResultContract>()
                .ForMember(dest => dest.BookId, opts => opts.MapFrom(src => src.Book.Id))
                .ForMember(dest => dest.BookXmlId, opts => opts.MapFrom(src => src.Book.Guid))
                .ForMember(dest => dest.VersionXmlId, opts => opts.MapFrom(src => src.VersionId))
                .ForMember(dest => dest.CreateTime, opts => opts.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.CreateTimeString, opts => opts.MapFrom(src => src.CreateTime.ToString(CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.Copyright, opts => opts.MapFrom(src => src.Copyright))
                .ForMember(dest => dest.PublishDate, opts => opts.MapFrom(src => src.PublishDate))
                .ForMember(dest => dest.PublishPlace, opts => opts.MapFrom(src => src.PublishPlace))
                .ForMember(dest => dest.Publisher, opts => opts.MapFrom(src => src.Publisher))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Title))
                .ForMember(dest => dest.SubTitle, opts => opts.MapFrom(src => src.SubTitle))
                .ForMember(dest => dest.Acronym, opts => opts.MapFrom(src => src.Acronym))
                .ForMember(dest => dest.BiblText, opts => opts.MapFrom(src => src.BiblText))
                .ForMember(dest => dest.BookType, opts => opts.MapFrom(src => src.Book.LastVersion.DefaultBookType.Type))
                .ForMember(dest => dest.Keywords, opts => opts.MapFrom(src => src.Keywords.Select(x => x.Text).ToList()))
                .ForMember(dest => dest.Manuscripts, opts => opts.MapFrom(src => src.ManuscriptDescriptions))
                .ForMember(dest => dest.Editors, opt => opt.MapFrom(src => src.Responsibles.Where(x => x.ResponsibleType.Type == ResponsibleTypeEnum.Editor))) //TODO add category
                .ForMember(dest => dest.RelicAbbreviation, opt => opt.MapFrom(src => src.RelicAbbreviation))
                .ForMember(dest => dest.SourceAbbreviation, opt => opt.MapFrom(src => src.SourceAbbreviation))
                .ForMember(dest => dest.LiteraryOriginals, opt => opt.MapFrom(src => src.LiteraryOriginals.Select(x=>x.Name)))
                .ForMember(dest => dest.LiteraryKinds, opt => opt.MapFrom(src => src.LiteraryKinds.Select(x=>x.Name)))
                .ForMember(dest => dest.LiteraryGenres, opt => opt.MapFrom(src => src.LiteraryGenres.Select(x=>x.Name)));
        }
    }
}