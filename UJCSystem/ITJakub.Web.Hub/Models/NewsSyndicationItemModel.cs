using System.ComponentModel.DataAnnotations;
using System.Web.Http.Results;

namespace ITJakub.Web.Hub.Models
{
    public class NewsSyndicationItemModel
    {
        [DataType(DataType.Text)]
        [Required]
        [Display(Name = "Název: ")]
        public string Title { get; set; }

        [DataType(DataType.Text)]
        [Required]
        [MaxLength(2000)]
        [Display(Name = "Text")]
        public string Content { get; set; }

        [Url]
        [Required]
        [DataType(DataType.Url)]
        [Display(Name = "Url: ")]        
        public string Url { get; set; }    
    }
}