using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class ResendConfirmCodeRequest
    {
        public ContactTypeContract ContactType { get; set; }
    }
}