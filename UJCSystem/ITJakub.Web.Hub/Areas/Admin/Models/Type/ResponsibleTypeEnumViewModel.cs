using System.ComponentModel.DataAnnotations;

namespace ITJakub.Web.Hub.Areas.Admin.Models.Type
{
    public enum ResponsibleTypeEnumViewModel
    {
        [Display(Name = "Neznámý")]
        Unknown = 0,

        [Display(Name = "Editor")]
        Editor = 1,

        [Display(Name = "Kolace")]
        Kolace = 2,
    }
}
