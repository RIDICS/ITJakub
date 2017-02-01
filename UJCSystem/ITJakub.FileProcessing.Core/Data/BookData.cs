using System;
using System.Collections.Generic;

namespace ITJakub.FileProcessing.Core.Data
{
    public class BookData
    {
        public string BookXmlId { get; set; }
        public string VersionXmlId { get; set; }
        public string VersionDescription { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public DateTime CreateTime { get; set; }
        public string Copyright { get; set; }
        public AvailabilityStatusEnum AvailabilityStatus { get; set; }
        public string BiblText { get; set; }
        public string RelicAbbreviation { get; set; }
        public string SourceAbbreviation { get; set; }
        public string PublishDate { get; set; }
        public string PublishPlace { get; set; }
        public PublisherData Publisher { get; set; }
        public List<ManuscriptDescriptionData> ManuscriptDescriptions { get; set; }
        public List<AuthorData> Authors { get; set; }
        public List<ResponsibleData> Responsibles { get; set; }
        public List<BookPageData> Pages { get; set; }
        public List<BookContentItemData> BookContentItems { get; set; }
        public List<BookHeadwordData> BookHeadwords { get; set; }
        public List<TrackData> Tracks { get; set; }
        public List<FullBookRecordingData> FullBookRecordings { get; set; }
        public List<CategoryData> AllCategoriesHierarchy { get; set; }
        public List<string> CategoryXmlIds { get; set; }
        public List<string> LiteraryGenres { get; set; }
        public List<string> LiteraryKinds { get; set; }
        public List<string> LiteraryOriginals { get; set; }
        public List<string> Keywords { get; set; }
        public List<TermData> Terms { get; set; }
        public List<BookAccessoryData> Accessories { get; set; }
        public List<TransformationData> Transformations { get; set; }
        public int BookContentItemsCount { get; set; }
    }
}