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
        [Display(Name = "Headword")]
        public string Headword { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Dictionary")]
        public string Dictionary { get; set; }
    }
}