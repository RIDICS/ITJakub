using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.RepositoryImport.Models
{
    public class CreateFilteringExpressionSetViewModel
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "BibliographicFormat")]
        public int BibliographicFormatId { get; set; }

        public IList<FilteringExpressionContract> FilteringExpressions { get; set; }
    }
}
