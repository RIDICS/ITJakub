namespace Vokabular.Authentication.DataContracts.User
{
    public class VerifiedUserCreatedContract : UserContractBase
    {
        public string Password { get; set; }

        public string UserName { get; set; }
    }
}