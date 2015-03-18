using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class BookInfoProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<BookVersion, BookInfoContract>()
                .ForMember(dest => dest.Guid, opts => opts.MapFrom(src => src.Book.Guid))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Title))
                .ForMember(dest => dest.SubTitle, opts => opts.MapFrom(src => src.SubTitle))
                .ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description))
                .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => src.PublishDate))
                .ForMember(dest => dest.PublishPlace, opts => opts.MapFrom(src => src.PublishPlace))
                .ForMember(dest => dest.BookPages, opt => opt.MapFrom(src => src.BookPages));
        }
    }
}