namespace Vokabular.Authentication.DataContracts.User
{
    public class ChangeUserContactsContract : ContractBase
    {
        public string NewEmailValue { get; set; }
        public string NewPhoneNumberValue { get; set; }
    }
}