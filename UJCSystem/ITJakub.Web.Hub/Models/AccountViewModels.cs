using System.ComponentModel.DataAnnotations;

namespace ITJakub.Web.Hub.Models
{
    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Uživatelské jméno nemůže být prázdné")]
        [DataType(DataType.Text)]
        [Display(Name = "Uživatelské jméno")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Heslo nemůže být prázdné")]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [Display(Name = "Zůstat přihlášen po vypnutí prohlížeče")]
        public bool RememberMe { get; set; }


    }

    public class RegisterViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Uživatelské jméno nemůže být prázdné")]
        [DataType(DataType.Text)]
        [Display(Name = "Uživatelské jméno")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email musí být vyplněn")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Heslo nemůže být prázdné")]
        [StringLength(100, ErrorMessage = "{0} musí obsahovat alespoň {2} znaků.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrzení hesla")]
        [Compare("Password", ErrorMessage = "Hesla se neshodují.")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Křestní jméno")]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Příjmení")]
        public string LastName { get; set; }
    }
}
