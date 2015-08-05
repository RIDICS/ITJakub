using System.Linq;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.SelectResults;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class BookProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Book, BookContract>()
                .ForMember(m => m.Title, opt => opt.MapFrom(src => src.LastVersion.Title))
                .ForMember(m => m.SubTitle, opt => opt.MapFrom(src => src.LastVersion.SubTitle))
                .ForMember(m => m.Guid, opt => opt.MapFrom(src => src.Guid))
                .ForMember(m => m.CategoryIds, opt => opt.MapFrom(src => src.LastVersion.Categories.Select(x => x.Id)));
        }
    }

    public class PageBookmarkProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<PageBookmark, PageBookmarkContract>()
                .ForMember(dest => dest.PagePosition, opt => opt.MapFrom(src => src.PagePosition))
                .ForMember(dest => dest.PageXmlId, opt => opt.MapFrom(src => src.PageXmlId));
        }
    }

    public class HeadwordBookmarkProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<HeadwordBookmarkResult, HeadwordBookmarkContract>()
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookGuid))
                .ForMember(dest => dest.EntryXmlId, opt => opt.MapFrom(src => src.XmlEntryId))
                .ForMember(dest => dest.Headword, opt => opt.MapFrom(src => src.Headword));
        }
    }
}