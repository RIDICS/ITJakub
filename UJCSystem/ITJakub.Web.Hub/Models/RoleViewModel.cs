using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ITJakub.Web.Hub.Models
{
    public class RoleViewModel
    {
        [ReadOnly(true)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "NotEmpty")]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public bool SuccessfulUpdate { get; set; }
    }
}
