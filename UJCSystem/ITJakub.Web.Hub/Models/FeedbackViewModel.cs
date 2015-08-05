using System.ComponentModel.DataAnnotations;

namespace ITJakub.Web.Hub.Models
{
    public class FeedbackViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Jméno: ")]
        public string Name { get; set; }

        [EmailAddress]
        [Display(Name = "Email: ")]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(2000)]
        [Display(Name = "Text")]
        public string Text { get; set; }
    }
}