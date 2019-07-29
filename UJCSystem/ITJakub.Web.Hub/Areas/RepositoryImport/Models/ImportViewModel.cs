using System.Collections.Generic;

namespace ITJakub.Web.Hub.Areas.RepositoryImport.Models
{
    public class ImportViewModel
    {
        public IList<CheckBoxEntity> ExternalRepositoryCheckBoxes { get; set; }
    }

    public class CheckBoxEntity
    {
        public CheckBoxEntity()
        {
        }

        public CheckBoxEntity(int id, string name, bool isChecked = false)
        {
            Id = id;
            Name = name;
            IsChecked = isChecked;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
}
