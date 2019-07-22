using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class ConfirmUserContactRequest
    {
        public ContactType ContactType { get; set; }

        public string ConfirmCode { get; set; }
    }
}