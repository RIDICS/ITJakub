namespace Vokabular.Authentication.DataContracts.User
{
    public class UserContract : UserContractBase
    {
        public string UserName { get; set; }

        public string VerificationCode { get; set; }
    }
}