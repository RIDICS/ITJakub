using System.Collections.Generic;

namespace ITJakub.Web.Hub.Areas.RepositoryImport.Models
{
    public class ImportViewModel
    {
        public IList<ExternalRepositoryCheckBox> ExternalRepositoryCheckBoxes { get; set; }
    }

    public class ExternalRepositoryCheckBox
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
}
