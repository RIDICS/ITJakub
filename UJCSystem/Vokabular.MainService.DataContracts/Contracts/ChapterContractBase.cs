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

    public class ChapterHierarchyContract : ChapterContractBase
    {
        public List<ChapterHierarchyContract> SubChapters { get; set; }
    }
}