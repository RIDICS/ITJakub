namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ImageContract
    {
        public long Id { get; set; }
        public long VersionId { get; set; }
        public int VersionNumber { get; set; }
        public string ImageUrl { get; set; }
    }
}