using System.ComponentModel.DataAnnotations;

namespace Ujc.Naki.CardFile.Web.Models
{
    public class CardFilesModel
    {
        [Display(Name = "Heslo")]
        public string Headword { get; set; }
    }
}