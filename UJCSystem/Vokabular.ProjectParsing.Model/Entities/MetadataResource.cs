using System;

namespace Vokabular.ProjectParsing.Model.Entities
{
    public class MetadataResource
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string AuthorsLabel { get; set; }
        public string RelicAbbreviation { get; set; }
        public string SourceAbbreviation { get; set; }
        public string PublishPlace { get; set; }
        public string PublishDate { get; set; }
        public string PublisherText { get; set; }
        public string PublisherEmail { get; set; }
        public string Copyright { get; set; }
        public string BiblText { get; set; }
        public string OriginDate { get; set; }
        public DateTime? NotBefore { get; set; }
        public DateTime? NotAfter { get; set; }
        public string ManuscriptIdno { get; set; }
        public string ManuscriptSettlement { get; set; }
        public string ManuscriptCountry { get; set; }
        public string ManuscriptRepository { get; set; }
        public string ManuscriptExtent { get; set; }
        public string ManuscriptTitle { get; set; }
        public string OriginalResourceUrl { get; set; }
    }
}
