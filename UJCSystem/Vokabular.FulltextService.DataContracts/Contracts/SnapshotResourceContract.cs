using System.Collections.Generic;

namespace Vokabular.FulltextService.DataContracts.Contracts
{
    public class SnapshotResourceContract
    {
        public long SnapshotId { get; set; }
        public string SnapshotText { get; set; }
        public long ProjectId { get; set; }
        public List<SnapshotPageResourceContract> Pages { get; set; }
    }

    public class SnapshotPageResourceContract
    {
        public string Id { get; set; }
        public int PageIndex { get; set; }
        public int indexFrom { get; set; }
        public int indexTo { get; set; }
        
    }
}