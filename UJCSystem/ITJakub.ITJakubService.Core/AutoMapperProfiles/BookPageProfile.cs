using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class BookPageProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<BookPage, BookPageContract>()
                .ForMember(m => m.XmlId, opt => opt.MapFrom(src => src.XmlId))
                .ForMember(m => m.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(m => m.Position, opt => opt.MapFrom(src => src.Position));
        }
    }
}