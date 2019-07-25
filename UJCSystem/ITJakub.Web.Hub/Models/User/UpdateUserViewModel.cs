using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ITJakub.Web.Hub.Models.User
{
    public class UpdateUserViewModel
    {
        [ReadOnly(true)]
        public int Id { get; set; }

        [Display(Name = "UserName")]
        [ReadOnly(true)]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "NotEmpty")]
        [DataType(DataType.Text)]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "NotEmpty")]
        [DataType(DataType.Text)]
        [Display(Name = "LastName")]
        public string LastName { get; set; }
    }
}
