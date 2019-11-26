using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class ProjectInfoViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ProjectTypeContract ProjectType { get; set; }
        public TextTypeEnumContract TextType { get; set; }
    }
}
