using System.Linq;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
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
                .ForMember(m => m.Guid, opt => opt.MapFrom(src => src.Guid));

            CreateMap<Book, BookContractWithCategories>()
                .ForMember(m => m.Title, opt => opt.MapFrom(src => src.LastVersion.Title))
                .ForMember(m => m.SubTitle, opt => opt.MapFrom(src => src.LastVersion.SubTitle))
                .ForMember(m => m.Guid, opt => opt.MapFrom(src => src.Guid))
                .ForMember(m => m.CategoryIds, opt => opt.MapFrom(src => src.LastVersion.Categories.Select(x => x.Id)));
        }
    }
}