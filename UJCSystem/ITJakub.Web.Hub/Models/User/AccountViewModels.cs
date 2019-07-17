using ITJakub.Web.Hub.Constants;

namespace ITJakub.Web.Hub.Models.User
{
    public class AccountDetailViewModel
    {
        public UpdateUserViewModel UpdateUserViewModel;

        public UpdatePasswordViewModel UpdatePasswordViewModel;

        public UpdateContactViewModel UpdateContactViewModel;

        public UpdateTwoFactorVerificationViewModel UpdateTwoFactorVerificationViewModel;

        public AccountTab ActualTab { get; set; }
    }
}
