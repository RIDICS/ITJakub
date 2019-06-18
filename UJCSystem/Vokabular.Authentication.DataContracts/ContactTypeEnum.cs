using System.ComponentModel.DataAnnotations;

namespace Vokabular.Authentication.DataContracts
{
    public enum ContactTypeEnum
    {
        [Display(Name = "email")]
        Email = 0,
        [Display(Name = "phone")]
        Phone = 1,
    }
}