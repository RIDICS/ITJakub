using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class ChapterEditorViewModel
    {
        public IList<ChapterHierarchyContract> Chapters { get; set; }
        public IList<PageContract> Pages { get; set; }
    }

    public class ChapterEditorRowViewModel
    {
        public ChapterHierarchyContract Chapter { get; set; }
        public IList<PageContract> Pages { get; set; }
    }
}
