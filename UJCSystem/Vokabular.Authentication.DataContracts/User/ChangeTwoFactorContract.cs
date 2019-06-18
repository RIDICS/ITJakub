namespace Vokabular.Authentication.DataContracts.User
{
    public class ChangeTwoFactorContract : ContractBase
    {
        public bool TwoFactorIsEnabled { get; set; }

        public string TwoFactorProvider { get; set; }
    }
}