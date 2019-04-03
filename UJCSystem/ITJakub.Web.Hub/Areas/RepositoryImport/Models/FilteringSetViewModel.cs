using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.RepositoryImport.Models
{
    public class FilteringSetViewModel
    {
        public SelectList AvailableBibliographicFormats { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "NotEmpty")]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        public int Id { get; set; }

        public FilteringExpressionSetDetailContract FilteringExpressionSet { get; set; }
    }
}
