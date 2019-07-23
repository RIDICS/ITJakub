using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class ConfirmUserContactRequest
    {
        public ContactTypeContract ContactType { get; set; }

        public string ConfirmCode { get; set; }
    }
}