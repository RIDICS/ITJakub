using System.ComponentModel.DataAnnotations;

namespace ITJakub.Web.Hub.Areas.Admin.Models.Type
{
    public class ResponsibleTypeViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public ResponsibleTypeEnumViewModel Type { get; set; }
    }

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
