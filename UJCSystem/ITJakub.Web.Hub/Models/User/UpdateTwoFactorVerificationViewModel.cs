using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ITJakub.Web.Hub.Models.User
{
    public class UpdateTwoFactorVerificationViewModel
    {
        [Display(Name = "TwoFactorEnabled")]
        public bool TwoFactorEnabled { get; set; }

        [Display(Name = "SelectedTwoFactorProvider")]
        public string SelectedTwoFactorProvider { get; set; }

        [Display(Name = "TwoFactorProviders")]
        public IList<string> TwoFactorProviders { get; set; }

        public bool IsEmailConfirmed { get; set; }
    }
}
