namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ChapterContract
    {
        public long Id { get; set; }
        public long VersionId { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public long BeginningPageId { get; set; }
    }
}