namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ExternalRepositoryContract
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }
        
        public virtual BibliographicFormatContract BibliographicFormat { get; set; }

        public virtual ExternalRepositoryTypeContract ExternalRepositoryType { get; set; }
    }

    public class ExternalRepositoryDetailContract : ExternalRepositoryContract
    {
        public virtual string Url { get; set; }

        public virtual string License { get; set; }

        public virtual string Configuration { get; set; }

        public virtual UserContract CreatedByUser { get; set; }
    }
}