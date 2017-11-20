namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ImageWithPageContract
    {
        public long Id { get; set; }
        public long VersionId { get; set; }
        public int VersionNumber { get; set; }
        public PageContract ParentPage { get; set; }
    }

    public class CreateImageContract
    {
        public string Comment { get; set; }
        public long OriginalVersionId { get; set; }
        public long ResourcePageId { get; set; }
        public string FileName { get; set; }
    }
}