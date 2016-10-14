using System.ComponentModel.DataAnnotations;

namespace ITJakub.Web.Hub.Models
{
    public class NewsSyndicationItemViewModel
    {
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "N�zev nem��e b�t pr�zdn�")]
        [Display(Name = "N�zev: ")]
        public string Title { get; set; }

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Text nem��e b�t pr�zdn�")]
        [MaxLength(2000)]
        [Display(Name = "Text: ")]
        public string Content { get; set; }

        [Url(ErrorMessage = "Vypln�n� URL nen� validn�. Mus� b�t zaps�na v �pln� form� podobn�ho form�tu http://nazev.domena")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "URL nem��e b�t pr�zdn�")]
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
        [Display(Name = "Web i mobiln� aplikace")]
        Combined = 0,
        
        [Display(Name = "Pouze web")]
        Web = 1,
        
        [Display(Name = "Pouze mobiln� aplikace")]
        MobileApps = 2,
    }


}