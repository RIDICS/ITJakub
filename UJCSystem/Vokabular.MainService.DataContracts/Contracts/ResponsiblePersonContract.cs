namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ResponsiblePersonContract
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ResponsibleTypeId { get; set; }
    }
}