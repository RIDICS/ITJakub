using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Constants;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace ITJakub.Web.Hub.Models
{
    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "NotEmpty")]
        [DataType(DataType.Text)]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "NotEmpty")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "NotEmpty")]
        [DataType(DataType.Text)]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "MustBeFilled")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "NotEmpty")]
        [StringLength(100, ErrorMessage = "HasAtLeastNChars", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessage = "PasswordsNotEqual")]
        public string ConfirmPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "NotEmpty")]
        [DataType(DataType.Text)]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "NotEmpty")]
        [DataType(DataType.Text)]
        [Display(Name = "LastName")]
        public string LastName { get; set; }
    }

    public class AccountDetailViewModel
    {
        public UpdateUserViewModel UpdateUserViewModel;

        public UpdatePasswordViewModel UpdatePasswordViewModel;

        public AccountTab ActualTab { get; set; }
    }

    public class UpdateUserViewModel
    {
        [ReadOnly(true)]
        public int Id { get; set; }

        [Display(Name = "UserName")]
        [ReadOnly(true)]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "MustBeFilled")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "NotEmpty")]
        [DataType(DataType.Text)]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "NotEmpty")]
        [DataType(DataType.Text)]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        public bool SuccessfulUpdate { get; set; }
    }

    public class UpdatePasswordViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "NotEmpty")]
        [DataType(DataType.Password)]
        [Display(Name = "OldPassword")]
        public string OldPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "NotEmpty")]
        [StringLength(100, ErrorMessage = "HasAtLeastNChars", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessage = "PasswordsNotEqual")]
        public string ConfirmPassword { get; set; }

        public bool SuccessfulUpdate { get; set; }
    }

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
