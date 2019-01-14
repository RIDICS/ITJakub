
namespace Vokabular.Authentication.DataContracts.User
{
    public class ChangePasswordContract : ContractBase
    {
        public string OriginalPassword { get; set; }

        public string Password { get; set; }
    }
}