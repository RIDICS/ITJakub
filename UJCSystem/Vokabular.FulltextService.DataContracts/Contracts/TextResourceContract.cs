namespace Vokabular.FulltextService.DataContracts.Contracts
{
    public class TextResourceContract
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string BookId { get; set; }
        public int VersionNumber { get; set; }
    }
}