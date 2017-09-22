namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ResponsiblePersonContract
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ProjectResponsiblePersonContract : ResponsiblePersonContract
    {
        public ResponsibleTypeContract ResponsibleType { get; set; }
    }

    public class ProjectResponsiblePersonIdContract
    {
        public long ResponsiblePersonId { get; set; }
        public long ResponsibleTypeId { get; set; }
    }
}