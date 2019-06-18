namespace Vokabular.Authentication.DataContracts.User
{
    public class CreateVerifiedUserContract : ContractBase
    {
        public UserContractBase User { get; set; }

        public string UserName { get; set; }
    }
}