using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace AuthService.Models
{

    public class AuthModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$",
            ErrorMessage = "Musíte vložit validní e-mailovou adresu")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$",
            ErrorMessage = "Musíte vložit validní e-mailovou adresu")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        public IEnumerable<string> Roles { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} musí být alespoň {2} znaků dlouhé.", MinimumLength = 6)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hesla se neshodují")]
        [Display(Name = "Heslo znovu")]
        public string PasswordAgain { get; set; }

        [Required]
        [Display(Name = "Křestní jméno")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Příjmení")]
        public string LastName { get; set; }
    }

    public class EditModel
    {
        public int Id { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        public IEnumerable<string> Roles { get; set; }

        [Required]
        [Display(Name = "Křestní jméno")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Příjmení")]
        public string LastName { get; set; }
    }
}
