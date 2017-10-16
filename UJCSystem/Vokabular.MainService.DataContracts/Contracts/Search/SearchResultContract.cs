using System;
using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.DataContracts.Contracts.Search
{
    public class SearchResultContract
    {
        public long BookId { get; set; }

        public string BookXmlId { get; set; }

        public string VersionXmlId { get; set; }

        public BookTypeEnumContract BookType { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        //public string Acronym { get; set; }

        public string BiblText { get; set; }

        public string OriginDate { get; set; }

        public DateTime? NotBefore { get; set; }

        public DateTime? NotAfter { get; set; }

        public string PublishPlace { get; set; }

        public string PublishDate { get; set; }

        public string PublisherText { get; set; }

        public string PublisherEmail { get; set; }

        public string AuthorsLabel { get; set; }
        
        //public PublisherContract Publisher { get; set; }

        public string Copyright { get; set; }

        public int PageCount { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateTimeString { get; set; }

        //public List<ManuscriptContract> Manuscripts { get; set; }

        public string ManuscriptIdno { get; set; }

        public string ManuscriptSettlement { get; set; }

        public string ManuscriptCountry { get; set; }

        public string ManuscriptRepository { get; set; }

        public string ManuscriptExtent { get; set; }

        public string ManuscriptTitle { get; set; }

        //public int TotalHitCount { get; set; } // Search hits in book (similar to Corpus search, but only in one book)

        //public IList<PageResultContext> Results { get; set; }

        public string RelicAbbreviation { get; set; }

        public string SourceAbbreviation { get; set; }

        public SearchTermResultContract TermPageHits { get; set; }
    }

    public class SearchResultDetailContract : SearchResultContract
    {
        public List<OriginalAuthorContract> Authors { get; set; }

        public List<string> Keywords { get; set; }

        public List<ProjectResponsiblePersonContract> Editors { get; set; }

        public IList<string> LiteraryOriginals { get; set; }

        public IList<string> LiteraryKinds { get; set; }

        public IList<string> LiteraryGenres { get; set; }
    }

    public class AudioBookSearchResultContract : SearchResultContract
    {
        public IList<TrackWithRecordingContract> Tracks { get; set; }

        public IList<AudioContract> FullBookRecordings { get; set; }
    }
}
