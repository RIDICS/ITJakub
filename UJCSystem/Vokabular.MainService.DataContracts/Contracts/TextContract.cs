namespace Vokabular.MainService.DataContracts.Contracts
{
    public class TextContract
    {
        public long Id { get; set; }
        public long VersionId { get; set; }
        public int VersionNumber { get; set; }
        public string ExternalId { get; set; }
        public long BookVersionId { get; set; }
    }

    public class FullTextContract : TextContract
    {
        public string Text { get; set; }
    }

    public class TextWithPageContract : TextContract
    {
        public PageContract ParentPage { get; set; }
    }
}