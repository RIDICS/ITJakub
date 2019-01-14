namespace Vokabular.Authentication.DataContracts.User
{
    public class CreateUserContract : ContractBase
    {
        public UserContractBase User { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}