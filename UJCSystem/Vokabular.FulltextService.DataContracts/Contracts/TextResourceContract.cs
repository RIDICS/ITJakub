namespace Vokabular.FulltextService.DataContracts.Contracts
{
    public class TextResourceContract
    {
        public string Id { get; set; }
        public string PageText { get; set; }
        public string BookId { get; set; }
        public int VersionNumber { get; set; }
    }
}