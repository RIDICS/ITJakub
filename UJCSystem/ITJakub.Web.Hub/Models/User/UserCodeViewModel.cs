using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ITJakub.Web.Hub.Models.User
{
    public class UserCodeViewModel
    {
        [Display(Name = "UserCode")]
        [ReadOnly(true)]
        public string UserCode { get; set; }
    }
}
