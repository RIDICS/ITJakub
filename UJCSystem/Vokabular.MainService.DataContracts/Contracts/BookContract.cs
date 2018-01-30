using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class BookContract : ProjectMetadataContract
    {
        public long Id { get; set; }
    }

    public class BookWithCategoriesContract : BookContract
    {
        public List<CategoryContract> CategoryList { get; set; }
    } 
}