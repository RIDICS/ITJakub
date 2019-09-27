namespace Vokabular.MainService.DataContracts.Contracts
{
    public class NewResourceResultContract
    {
        public long ResourceId { get; set; }
        public long ResourceVersionId { get; set; }
        public int VersionNumber { get; set; }
    }
}