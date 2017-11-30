using System.Linq;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class BookInfoProfile : Profile
    {
        public BookInfoProfile()
        {
            CreateMap<BookVersion, BookInfoWithPagesContract>()
                .ForMember(dest => dest.BookId, opts => opts.MapFrom(src => src.Book.Id))
                .ForMember(dest => dest.BookXmlId, opts => opts.MapFrom(src => src.Book.Guid))
                .ForMember(dest => dest.LastVersionXmlId, opts => opts.MapFrom(src => src.VersionId))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Title))
                .ForMember(dest => dest.SubTitle, opts => opts.MapFrom(src => src.SubTitle))
                .ForMember(dest => dest.Acronym, opts => opts.MapFrom(src => src.Acronym))
                .ForMember(dest => dest.BiblText, opts => opts.MapFrom(src => src.BiblText))
                .ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description))
                .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => src.PublishDate))
                .ForMember(dest => dest.PublishPlace, opts => opts.MapFrom(src => src.PublishPlace))
                .ForMember(dest => dest.BookPages, opt => opt.MapFrom(src => src.BookPages));

            CreateMap<BookVersion, BookContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Book.Id))
                .ForMember(dest => dest.Guid, opts => opts.MapFrom(src => src.Book.Guid))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Title))
                .ForMember(dest => dest.SubTitle, opts => opts.MapFrom(src => src.SubTitle));

            CreateMap<BookVersion, BookContractWithCategories>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Book.Id))
                .ForMember(dest => dest.Guid, opts => opts.MapFrom(src => src.Book.Guid))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Title))
                .ForMember(dest => dest.SubTitle, opts => opts.MapFrom(src => src.SubTitle))
                .ForMember(dest => dest.CategoryIds, opts => opts.MapFrom(src => src.Categories.Select(x => x.Id)));

            CreateMap<BookVersion, MobileApps.MobileContracts.BookContract>()
                .ForMember(dest => dest.Guid, opts => opts.MapFrom(src => src.Book.Guid))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors))
                .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => src.PublishDate));
        }
    }
}