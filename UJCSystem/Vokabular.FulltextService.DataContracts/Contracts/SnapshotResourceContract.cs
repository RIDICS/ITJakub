namespace Vokabular.FulltextService.DataContracts.Contracts
{
    public class SnapshotResourceContract
    {
        public long SnapshotId { get; set; }
        public string SnapshotText { get; set; }
        public long ProjectId { get; set; }
    }
}