using System.Linq;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using Vokabular.Shared.DataContracts.Search.Old;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class CorpusSearchResultContractProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<BookVersion, CorpusSearchResultContract>()
                .ForMember(dest => dest.BookId, opts => opts.MapFrom(src => src.Book.Id))
                .ForMember(dest => dest.BookXmlId, opts => opts.MapFrom(src => src.Book.Guid))
                .ForMember(dest => dest.VersionXmlId, opts => opts.MapFrom(src => src.VersionId))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Title))
                .ForMember(dest => dest.Author, opts => opts.MapFrom(src => src.Authors.FirstOrDefault() != null ? src.Authors.First().Name : null))
                .ForMember(dest => dest.Acronym, opts => opts.MapFrom(src => src.Acronym))
                .ForMember(dest => dest.OriginDate, opts => opts.MapFrom(src => src.ManuscriptDescriptions.FirstOrDefault() != null ? src.ManuscriptDescriptions.First().OriginDate : null));
        }
    }
}