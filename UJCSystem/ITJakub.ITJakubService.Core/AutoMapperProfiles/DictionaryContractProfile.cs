using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class DictionaryContractProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<BookVersion, DictionaryContract>()
                .ForMember(dest => dest.BookId, opts => opts.MapFrom(src => src.Book.Id))
                .ForMember(dest => dest.BookXmlId, opts => opts.MapFrom(src => src.Book.Guid))
                .ForMember(dest => dest.BookVersionId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.BookVersionXmlId, opts => opts.MapFrom(src => src.VersionId))
                .ForMember(dest => dest.BookAcronym, opts => opts.MapFrom(src => src.Acronym ?? src.SourceAbbreviation))
                .ForMember(dest => dest.BookTitle, opts => opts.MapFrom(src => src.Title));
        }
    }
}