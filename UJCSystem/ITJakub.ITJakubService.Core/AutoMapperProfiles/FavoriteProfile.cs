using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Entities.SelectResults;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.ITJakubService.DataContracts.Contracts.Favorite;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class FavoriteProfile : Profile
    {
        public FavoriteProfile()
        {
            CreateMap<FavoriteBase, FavoriteBaseInfoContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Title))
                .ForMember(dest => dest.FavoriteType, opts => opts.MapFrom(src => src.FavoriteTypeEnum))
                .ForMember(dest => dest.CreateTime, opts => opts.MapFrom(src => src.CreateTime));

            CreateMap<FavoriteBase, FavoriteBaseDetailContract>()
                .ForMember(dest => dest.FavoriteLabel, opts => opts.MapFrom(src => src.FavoriteLabel));

            CreateMap<FavoriteBase, FavoriteFullInfoContract>();

            CreateMap<FavoriteQuery, FavoriteQueryContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Title))
                .ForMember(dest => dest.CreateTime, opts => opts.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.Query, opts => opts.MapFrom(src => src.Query))
                .ForMember(dest => dest.BookType, opts => opts.MapFrom(src => src.BookType.Type))
                .ForMember(dest => dest.QueryType, opts => opts.MapFrom(src => src.QueryType))
                .ForMember(dest => dest.FavoriteLabel, opts => opts.MapFrom(src => src.FavoriteLabel));


            CreateMap<FavoriteTypeEnum, FavoriteTypeContract>()
                .ReverseMap();
        }
    }

    public class PageBookmarkProfile : Profile
    {
        public PageBookmarkProfile()
        {
            CreateMap<PageBookmark, PageBookmarkContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PagePosition, opt => opt.MapFrom(src => src.PagePosition))
                .ForMember(dest => dest.PageXmlId, opt => opt.MapFrom(src => src.PageXmlId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.FavoriteLabel, opt => opt.MapFrom(src => src.FavoriteLabel));
        }
    }

    public class HeadwordBookmarkProfile : Profile
    {
        public HeadwordBookmarkProfile()
        {
            CreateMap<HeadwordBookmarkResult, HeadwordBookmarkContract>()
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookGuid))
                .ForMember(dest => dest.EntryXmlId, opt => opt.MapFrom(src => src.XmlEntryId))
                .ForMember(dest => dest.Headword, opt => opt.MapFrom(src => src.Headword));
        }
    }
}
