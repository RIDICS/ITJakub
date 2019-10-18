using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ChapterContractBase
    {
        public long Id { get; set; }
        public long VersionId { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public long BeginningPageId { get; set; }
    }

    public class GetChapterContract : ChapterContractBase
    {
        public long? ParentChapterId { get; set; }
    }
    
    public class ChapterHierarchyContract : ChapterContractBase
    {
        public List<ChapterHierarchyContract> SubChapters { get; set; }
    }
    
    public class ChapterHierarchyDetailContract : ChapterContractBase
    {
        public List<ChapterHierarchyDetailContract> SubChapters { get; set; }
        public string BeginningPageName { get; set; }
    }

    public class CreateChapterContract
    {
        public string Comment { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public long BeginningPageId { get; set; }
        public long? ParentChapterId { get; set; }
    }

    public class CreateOrUpdateChapterContract : CreateChapterContract
    {
        public long? Id { get; set; }
    }
}