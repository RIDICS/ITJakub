using System.ComponentModel.DataAnnotations;
using System.Web.Http.Results;
using ITJakub.Shared.Contracts.News;

namespace ITJakub.Web.Hub.Models
{
    public class NewsSyndicationItemViewModel
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
        
        [EnumDataType(typeof(NewsTypeContract))]
        [Required]
        [Display(Name="Type: ")]
        public NewsTypeContract ItemType { get; set; }
    }

   
}