using System.ComponentModel.DataAnnotations;
using ITJakub.Web.Hub.Models;

namespace ITJakub.Web.Hub.Areas.Dictionaries.Models
{
    public class HeadwordFeedbackViewModel : FeedbackViewModel
    {
        [DataType(DataType.Text)]
        public long? BookId { get; set; }

        [DataType(DataType.Text)]
        public long? HeadwordVersionId { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Heslo: ")]
        public string Headword { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Slovník: ")]
        public string Dictionary { get; set; }
    }
}