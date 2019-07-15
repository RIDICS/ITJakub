namespace Vokabular.ProjectParsing.Model.Entities
{
    public class ProjectMetadata
    {
        public ProjectMetadata()
        {
            ManuscriptDescriptionData = new ManuscriptDescriptionData();
        }

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
        public ManuscriptDescriptionData ManuscriptDescriptionData { get; set; }
        public string OriginalResourceUrl { get; set; }
    }
}
