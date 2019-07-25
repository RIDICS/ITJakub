using System.ComponentModel.DataAnnotations;
using ITJakub.Web.Hub.Areas.Admin.Models;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace ITJakub.Web.Hub.Models.User
{ 
    public class UserDetailViewModel
    {
        public int Id { get; set; }

        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Display(Name = "Roles")]
        public ListViewModel<RoleContract> Roles { get; set; }
    }
}
