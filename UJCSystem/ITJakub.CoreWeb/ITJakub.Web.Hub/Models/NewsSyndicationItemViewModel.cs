using System.ComponentModel.DataAnnotations;

namespace ITJakub.Web.Hub.Models
{
    public class NewsSyndicationItemViewModel
    {
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Název nemùže být prázdný")]
        [Display(Name = "Název: ")]
        public string Title { get; set; }

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Text nemùže být prázdný")]
        [MaxLength(2000)]
        [Display(Name = "Text: ")]
        public string Content { get; set; }

        [Url(ErrorMessage = "Vyplnìná URL není validní. Musí být zapsána v úplné formì podobného formátu http://nazev.domena")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "URL nemùže být prázdná")]
        [DataType(DataType.Url)]
        [Display(Name = "Url: ")]        
        public string Url { get; set; }    
        
        [EnumDataType(typeof(NewsTypeContractViewEnum))]
        [Required]
        [Display(Name="Typ: ")]
        public NewsTypeContractViewEnum ItemType { get; set; }
    }

    public enum NewsTypeContractViewEnum
    {
        [Display(Name = "Web i mobilní aplikace")]
        Combined = 0,
        
        [Display(Name = "Pouze web")]
        Web = 1,
        
        [Display(Name = "Pouze mobilní aplikace")]
        MobileApps = 2,
    }


}