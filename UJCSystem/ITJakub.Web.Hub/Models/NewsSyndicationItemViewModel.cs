using System.ComponentModel.DataAnnotations;

namespace ITJakub.Web.Hub.Models
{
    public class NewsSyndicationItemViewModel
    {
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "NameNotEmpty")]
        [Display(Name = "Name:")]
        public string Title { get; set; }

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "TextNotEmpty")]
        [MaxLength(2000)]
        [Display(Name = "Text:")]
        public string Content { get; set; }

        [Url(ErrorMessage = "UrlNotValid")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "UrlNotEmpty")]
        [DataType(DataType.Url)]
        [Display(Name = "Url:")]        
        public string Url { get; set; }    
        
        [EnumDataType(typeof(NewsTypeContractViewEnum))]
        [Required]
        //[Display(Name="Type")]
        public NewsTypeContractViewEnum ItemType { get; set; }
    }

    public enum NewsTypeContractViewEnum
    {
        [Display(Name = "WebAndMobileApp")]
        Combined = 0,
        
        [Display(Name = "OnlyWeb")]
        Web = 1,
        
        [Display(Name = "OnlyMobileApp")]
        MobileApps = 2,
    }


}