using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
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
                .ForMember(m => m.SubTitle, opt => opt.MapFrom(src => src.LastVersion.SubTitle));

            CreateMap<Book, MobileApps.MobileContracts.BookContract>()
                .ForMember(m => m.Title, opt => opt.MapFrom(src => src.LastVersion.Title))
                .ForMember(m => m.Authors, opt => opt.MapFrom(src => src.LastVersion.Authors))
                .ForMember(m => m.PublishDate, opt => opt.MapFrom(src => src.LastVersion.PublishDate));
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
}