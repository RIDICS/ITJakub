namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ImageWithPageContract
    {
        public long Id { get; set; }
        public long VersionId { get; set; }
        public int VersionNumber { get; set; }
        public PageContract ParentPage { get; set; }
    }
}