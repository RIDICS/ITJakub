using System.Linq;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching.Results;
using ResponsibleType = ITJakub.DataEntities.Database.Entities.Enums.ResponsibleType;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class SearchResultContractProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<BookVersion, SearchResultContract>()
                .ForMember(dest => dest.BookType, opts => opts.MapFrom(src => src.Book.LastVersion.DefaultBookType.Type)) //TODO change to booktype list (from categories)
                .ForMember(dest => dest.PageCount, opts => opts.MapFrom(src => src.BookPages.Count))
                .ForMember(dest => dest.Keywords, opts => opts.MapFrom(src => src.Keywords.Select(x => x.Text).ToList()))
                .ForMember(dest => dest.Manuscripts, opts => opts.MapFrom(src => src.ManuscriptDescriptions))
                .ForMember(dest => dest.Editors, opt => opt.MapFrom(src => src.Responsibles.Where(x => x.ResponsibleType.Type == ResponsibleType.Editor))); //TODO add category
        }
    }
}