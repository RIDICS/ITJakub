using System.Globalization;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using Vokabular.Shared.DataContracts.Search.Old;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class AudioBookSearchResultContractProfile : Profile
    {
        public AudioBookSearchResultContractProfile()
        {
            CreateMap<BookVersion, AudioBookSearchResultContract>()
                .ForMember(dest => dest.BookId, opts => opts.MapFrom(src => src.Book.Id))
                .ForMember(dest => dest.BookXmlId, opts => opts.MapFrom(src => src.Book.Guid))
                .ForMember(dest => dest.VersionXmlId, opts => opts.MapFrom(src => src.VersionId))
                .ForMember(dest => dest.CreateTime, opts => opts.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.CreateTimeString, opts => opts.MapFrom(src => src.CreateTime.ToString(CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.Copyright, opts => opts.MapFrom(src => src.Copyright))
                .ForMember(dest => dest.PublishDate, opts => opts.MapFrom(src => src.PublishDate))
                .ForMember(dest => dest.PublishPlace, opts => opts.MapFrom(src => src.PublishPlace))
                .ForMember(dest => dest.Publisher, opts => opts.MapFrom(src => src.Publisher))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Title))
                .ForMember(dest => dest.SubTitle, opts => opts.MapFrom(src => src.SubTitle))
                .ForMember(dest => dest.Acronym, opts => opts.MapFrom(src => src.Acronym))
                .ForMember(dest => dest.SourceAbbreviation, opts => opts.MapFrom(src => src.SourceAbbreviation))
                .ForMember(dest => dest.RelicAbbreviation, opts => opts.MapFrom(src => src.RelicAbbreviation))
                .ForMember(dest => dest.BiblText, opts => opts.MapFrom(src => src.BiblText))
                .ForMember(dest => dest.BookType, opts => opts.MapFrom(src => src.Book.LastVersion.DefaultBookType.Type))
                .ForMember(dest => dest.Manuscripts, opts => opts.MapFrom(src => src.ManuscriptDescriptions))
                .ForMember(dest => dest.Tracks, opt => opt.Ignore())    //tracks are mapped individually
                .ForMember(dest => dest.FullBookRecordings, opt => opt.Ignore());  // mapped individually
                

            CreateMap<FullBookRecording, RecordingContract>()
                .ForMember(dest => dest.AudioType, opts => opts.MapFrom(src => src.AudioType))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType));

            CreateMap<Track, TrackContract>()
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
                .ForMember(dest => dest.Text, opts => opts.MapFrom(src => src.Text))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.Recordings, opt => opt.MapFrom(src => src.Recordings));

            CreateMap<TrackRecording, TrackRecordingContract>()
                .ForMember(dest => dest.Length, opts => opts.MapFrom(src => src.Length));
        }
    }
}