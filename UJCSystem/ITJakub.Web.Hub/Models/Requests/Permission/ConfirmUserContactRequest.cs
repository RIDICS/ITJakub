using Ridics.Authentication.DataContracts;

namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class ConfirmUserContactRequest
    {
        public int UserId { get; set; }

        public ContactTypeEnum ContactType { get; set; }

        public string ConfirmCode { get; set; }
    }
}