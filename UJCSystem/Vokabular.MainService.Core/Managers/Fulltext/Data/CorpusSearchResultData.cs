using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Search.Corpus;

namespace Vokabular.MainService.Core.Managers.Fulltext.Data
{
    public class CorpusSearchResultData
    {
        public long ProjectId { get; set; }

        public string ProjectExternalId { get; set; }
        
        public IList<string> Notes { get; set; }

        public CorpusSearchPageResultData PageResultContext { get; set; }

        public VerseResultContext VerseResultContext { get; set; }

        public BibleVerseResultContext BibleVerseResultContext { get; set; }
    }

    public class CorpusSearchResultDataList
    {
        public FulltextSearchResultType SearchResultType { get; set; }
        
        public List<CorpusSearchResultData> List { get; set; }
    }

    public class CorpusSearchPageResultData
    {
        public long TextId { get; set; }

        public string TextExternalId { get; set; }

        public KwicStructure ContextStructure { get; set; }
    }
}