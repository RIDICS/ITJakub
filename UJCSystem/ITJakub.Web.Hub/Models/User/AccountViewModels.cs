using ITJakub.Web.Hub.Constants;

namespace ITJakub.Web.Hub.Models.User
{
    public class AccountDetailViewModel
    {
        public UpdateUserViewModel UpdateUserViewModel { get; set; }

        public UpdatePasswordViewModel UpdatePasswordViewModel { get; set; }

        public UpdateContactViewModel UpdateContactViewModel { get; set; }

        public UpdateTwoFactorVerificationViewModel UpdateTwoFactorVerificationViewModel { get; set; }
        
        public UserCodeViewModel UserCodeViewModel { get; set; }
        
        public UserRolesViewModel UserRolesViewModel { get; set; }

        public AccountTab ActualTab { get; set; }
    }
}
