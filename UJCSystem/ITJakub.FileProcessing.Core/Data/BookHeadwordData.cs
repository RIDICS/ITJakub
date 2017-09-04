namespace ITJakub.FileProcessing.Core.Data
{
    public class BookHeadwordData
    {
        public string XmlEntryId { get; set; }
        public string DefaultHeadword { get; set; }
        public string Headword { get; set; }
        public string HeadwordOriginal { get; set; }
        public string Transliterated { get; set; }
        public string SortOrder { get; set; }
        public string Image { get; set; }
        public VisibilityEnum Visibility { get; set; }
    }
}