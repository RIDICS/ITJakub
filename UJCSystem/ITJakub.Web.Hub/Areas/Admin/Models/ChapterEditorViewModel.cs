using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class ChapterEditorViewModel
    {
        public IList<ChapterHierarchyDetailContract> Chapters { get; set; }
        public IList<PageContract> Pages { get; set; }
    }

    public class ChapterEditorRowViewModel
    {
        public ChapterHierarchyDetailContract Chapter { get; set; }
        public IList<PageContract> Pages { get; set; }
        
        public int LevelOfHierarchy { get; set; }
    }
}
