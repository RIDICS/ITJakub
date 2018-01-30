using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class CategoryContract
    {
        public int Id { get; set; }

        public int? ParentCategoryId { get; set; }

        public string ExternalId { get; set; }

        public string Description { get; set; }
    }

    public class CategoryTreeContract : CategoryContract
    {
        public List<CategoryTreeContract> Subcategories { get; set; }
    }
}