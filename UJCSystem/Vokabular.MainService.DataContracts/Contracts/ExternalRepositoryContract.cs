namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ExternalRepositoryContract
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        
        public BibliographicFormatContract BibliographicFormat { get; set; }

        public ExternalRepositoryTypeContract ExternalRepositoryType { get; set; }
    }

    public class ExternalRepositoryDetailContract : ExternalRepositoryContract
    {
        public string Url { get; set; }

        public string License { get; set; }

        public string Configuration { get; set; }

        public UserContract CreatedByUser { get; set; }
    }
}