namespace Vokabular.MainService.DataContracts.Contracts
{
    public class PageContract
    {
        public long Id { get; set; }
        public long VersionId { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
    }

    public class CreatePageContract
    {
        public string Name { get; set; }
        public int Position { get; set; }
        public string Comment { get; set; }
    }

    public class CreateOrUpdatePageContract
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
    }
}