using Ridics.Authentication.DataContracts;

namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class ResendConfirmCodeRequest
    {
        public int UserId { get; set; }

        public ContactTypeEnum ContactType { get; set; }
    }
}