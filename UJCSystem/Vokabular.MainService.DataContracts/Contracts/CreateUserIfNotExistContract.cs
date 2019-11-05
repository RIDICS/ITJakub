namespace Vokabular.MainService.DataContracts.Contracts
{
    public class CreateUserIfNotExistContract
    {
        public int ExternalId { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}