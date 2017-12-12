using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.OldGrammar.Models
{
    public class TermCategoriesWithTermsModel
    {
        public IList<TermCategoryDetailContract> TermCategories { get; set; }
    }
}