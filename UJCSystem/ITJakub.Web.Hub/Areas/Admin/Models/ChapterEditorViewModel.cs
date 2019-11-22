using System.Collections.Generic;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class ChapterEditorViewModel
    {
        public IList<ChapterHierarchyViewModel> Chapters { get; set; }
        public IList<PageViewModel> Pages { get; set; }
    }

    public class ChapterEditorRowViewModel
    {
        public ChapterHierarchyViewModel Chapter { get; set; }
        public IList<PageViewModel> Pages { get; set; }
        
        public int LevelOfHierarchy { get; set; }
    }
    
    public class ChapterHierarchyViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public long BeginningPageId { get; set; }
        public List<ChapterHierarchyViewModel> SubChapters { get; set; }
        public string BeginningPageName { get; set; }   
    }
}
