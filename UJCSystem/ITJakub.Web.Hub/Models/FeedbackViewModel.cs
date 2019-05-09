using System.ComponentModel.DataAnnotations;

namespace ITJakub.Web.Hub.Models
{
    public class FeedbackViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Name:")]
        [Required(ErrorMessage = "NameNotFilled")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "EmailWrongFormatError")]
        [Display(Name = "Email:")]
        [Required(ErrorMessage = "NameNotFilled1")]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(2000)]
        [Display(Name = "Text")]
        [Required(ErrorMessage = "FeedbackNotFilled")]
        public string Text { get; set; }

        public StaticTextViewModel PageStaticText { get; set; }

        public FeedbackFormIdentification FormIdentification { get; set; }
    }

    public class FeedbackFormIdentification
    {
        public string Area { get; set; }

        public string Controller { get; set; }
    }
}