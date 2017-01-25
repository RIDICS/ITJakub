using System;
using System.Collections.Generic;

namespace ITJakub.FileProcessing.Core.Data
{
    public class BookData
    {
        public string BookXmlId { get; set; }
        public string VersionXmlId { get; set; }
        public string VersionDescription { get; set; }
        public DateTime CreateTime { get; set; }
        public string Copyright { get; set; }
        public AvailabilityStatus AvailabilityStatus { get; set; }
        public IList<BookAccessoryData> Accessories { get; set; }
        public IList<BookPageData> Pages { get; set; }
        public IList<TrackData> Tracks { get; set; }
        public List<BookContentItemData> BookContentItems { get; set; }
        public List<AuthorData> Authors { get; set; }
        public string BiblText { get; set; }
        public string RelicAbbreviation { get; set; }
        public string SourceAbbreviation { get; set; }
        public List<KeywordData> Keywords { get; set; }
        public List<ManuscriptDescriptionData> ManuscriptDescriptions { get; set; }
        public PublisherData Publisher { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string PublishDate { get; set; }
        public string PublishPlace { get; set; }
        public List<ResponsibleData> Responsibles { get; set; }
        public List<LiteraryGenreData> LiteraryGenres { get; set; }
        public List<LiteraryKindData> LiteraryKinds { get; set; }
        public List<LiteraryOriginalData> LiteraryOriginals { get; set; }
        public List<BookHeadwordData> BookHeadwords { get; set; }
    }
}