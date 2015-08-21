using System.Globalization;
using System.Linq;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts.Searching.Results;
using ResponsibleType = ITJakub.DataEntities.Database.Entities.Enums.ResponsibleType;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class AudioBookSearchResultContractProfile : Profile
    {
        protected override void Configure()
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
                .ForMember(dest => dest.BiblText, opts => opts.MapFrom(src => src.BiblText))
                .ForMember(dest => dest.BookType, opts => opts.MapFrom(src => src.Book.LastVersion.DefaultBookType.Type))
                .ForMember(dest => dest.Keywords, opts => opts.MapFrom(src => src.Keywords.Select(x => x.Text).ToList()))
                .ForMember(dest => dest.Manuscripts, opts => opts.MapFrom(src => src.ManuscriptDescriptions))
                .ForMember(dest => dest.Editors, opt => opt.MapFrom(src => src.Responsibles.Where(x => x.ResponsibleType.Type == ResponsibleType.Editor)))
                .ForMember(dest => dest.FullBookRecordings, opt => opt.MapFrom(src => src.FullBookRecordings))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.Tracks.ToList()));

            CreateMap<FullBookRecording, RecordingContract>()
                .ForMember(dest => dest.AudioType, opts => opts.MapFrom(src => src.AudioType))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType));

            CreateMap<Track, TrackContract>()
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.Recordings, opt => opt.MapFrom(src => src.Recordings.ToList()));

            CreateMap<TrackRecording, TrackRecordingContract>()
                .ForMember(dest => dest.Length, opts => opts.MapFrom(src => src.Length));
        }
    }
}