using System.ComponentModel.DataAnnotations;

namespace ITJakub.Web.Hub.Models
{
    public class FeedbackViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Jméno: ")]
        [Required(ErrorMessage = "Jméno uživatele nebylo vyplněno.")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Nesprávný formát zadaného e-mailu.")]
        [Display(Name = "Email: ")]
        [Required(ErrorMessage = "E-mail uživatele nebyl vyplněn.")]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(2000)]
        [Display(Name = "Text")]
        [Required(ErrorMessage = "Text připomínky nebyl vyplněn.")]
        public string Text { get; set; }

        public StaticTextViewModel PageStaticText { get; set; }
    }
}