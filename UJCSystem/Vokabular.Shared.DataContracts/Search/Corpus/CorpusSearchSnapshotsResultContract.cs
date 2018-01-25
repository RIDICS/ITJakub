using System.Collections.Generic;

namespace Vokabular.Shared.DataContracts.Search.Corpus
{
    public class CorpusSearchSnapshotsResultContract
    {
        public List<long> SnapshotIds { get; set; }

        public long TotalCount { get; set; }
    
    }
}