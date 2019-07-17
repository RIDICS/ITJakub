using System.ComponentModel.DataAnnotations;

namespace ITJakub.Web.Hub.Models.User
{
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
    }
}
