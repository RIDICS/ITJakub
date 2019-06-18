namespace Vokabular.Authentication.DataContracts
{
    public class ContactContract : ContractBase
    {
        public int UserId { get; set; }

        public ContactTypeEnum ContactType { get; set; }

        public string ContactValue { get; set; }
    }
}