using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ITJakub.Web.Hub.Areas.RepositoryImport.Models
{
    public class CreateExternalRepositoryViewModel
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "Name")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Display(Name = "Description")] 
        public string Description { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "Url")] 
        public string Url { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "UrlTemplate")]
        public string UrlTemplate { get; set; }

        [Display(Name = "License")] 
        public string License { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "ApiTypeRequired")]
        [Display(Name = "ApiType")]
        public int ExternalRepositoryTypeId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "BibliographicFormat")]
        public int BibliographicFormatId { get; set; }

        [Display(Name = "FilteringExpressionSets")]
        public IList<CheckBoxEntity> FilteringExpressionSets { get; set; }

        public string Configuration { get; set; }
    }
}
