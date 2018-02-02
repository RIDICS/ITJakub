using System.Collections.Generic;

namespace Vokabular.Shared.DataContracts.Search.Corpus
{
    public class CorpusSearchSnapshotsResultContract
    {
        public List<CorpusSearchSnapshotContract> SnapshotList { get; set; }

        public long TotalCount { get; set; }
    }

    public class CorpusSearchSnapshotContract
    {
        public long SnapshotId { get; set; }

        public long ResultCount { get; set; }

        //TODO maybe add book title etc...
    }
}