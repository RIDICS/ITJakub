namespace ITJakub.DataEntities.Database.Entities.SelectResults
{
    public class HeadwordSearchResult
    {
        public string XmlEntryId { get; set; }
        public string Headword { get; set; }
        public string BookGuid { get; set; }
        public string BookVersionId { get; set; }
        public string BookAcronym { get; set; }
        public string BookTitle { get; set; }
        public string Image { get; set; }
        public string SortOrder { get; set; }
    }
}