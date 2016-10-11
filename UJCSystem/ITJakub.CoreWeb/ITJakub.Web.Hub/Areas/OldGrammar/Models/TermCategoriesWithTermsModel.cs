using System.Collections.Generic;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.Web.Hub.Areas.OldGrammar.Models
{
    public class TermCategoriesWithTermsModel
    {
        public IList<TermCategoryContract> TermCategories { get; set; }
    }
}