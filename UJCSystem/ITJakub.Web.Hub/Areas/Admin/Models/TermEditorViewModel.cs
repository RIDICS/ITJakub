using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class TermEditorViewModel
    {
        public IList<TermCategoryDetailContract> TermCategories { get; set; }
        public IList<PageContract> Pages { get; set; }
    }
}
