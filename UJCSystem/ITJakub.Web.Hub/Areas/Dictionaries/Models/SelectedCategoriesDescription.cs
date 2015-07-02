using System.Collections.Generic;

namespace ITJakub.Web.Hub.Areas.Dictionaries.Models
{
    public class SelectedCategoriesDescription
    {
        public string Type { get; set; }

        public IList<SelectedItemDescription> SelectedItems { get; set; }

        public IList<SelectedItemDescription> SelectedCategories { get; set; }
    }
    
    public class SelectedItemDescription
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}