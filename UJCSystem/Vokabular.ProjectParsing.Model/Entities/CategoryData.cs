using System.Collections.Generic;

namespace Vokabular.ProjectParsing.Model.Entities
{
    public class CategoryData
    {
        public string Description { get; set; }
        public IList<CategoryData> SubCategories { get; set; }
    }
}
