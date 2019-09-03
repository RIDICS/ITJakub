using System.Collections.Generic;

namespace ITJakub.Web.Hub.DataContracts
{
    public class CorpusSearchTotalResultCountBase
    {
        public IList<long> SelectedSnapshotIds { get; set; }
        public IList<int> SelectedCategoryIds { get; set; }
    }

    public class CorpusSearchTotalResultCountBasic : CorpusSearchTotalResultCountBase
    {
        public string Text { get; set; }
    }


    public class CorpusSearchTotalResultCountAdvanced : CorpusSearchTotalResultCountBase
    {
        public string Json { get; set; }
    }
}