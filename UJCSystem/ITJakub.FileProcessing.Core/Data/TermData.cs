namespace ITJakub.FileProcessing.Core.Data
{
    public class TermData
    {
        public string XmlId { get; set; }
        public long Position { get; set; }
        public string Text { get; set; }
        public TermCategoryData TermCategory { get; set; }
    }
}