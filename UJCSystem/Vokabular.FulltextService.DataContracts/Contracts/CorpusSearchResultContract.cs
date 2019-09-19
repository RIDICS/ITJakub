using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Search.Corpus;

namespace Vokabular.FulltextService.DataContracts.Contracts
{
    public class CorpusSearchResultContract
    {
        public long ProjectId { get; set; }

        public IList<string> Notes { get; set; }

        public CorpusSearchPageResultContract PageResultContext { get; set; }

        public VerseResultContext VerseResultContext { get; set; }

        public BibleVerseResultContext BibleVerseResultContext { get; set; }
    }

    public class CorpusSearchPageResultContract
    {
        public string TextExternalId { get; set; }

        public KwicStructure ContextStructure { get; set; }
    }
}
