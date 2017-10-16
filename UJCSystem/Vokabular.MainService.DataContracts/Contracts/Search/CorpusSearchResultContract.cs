using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Search.Corpus;

namespace Vokabular.MainService.DataContracts.Contracts.Search
{
    public class CorpusSearchResultContract
    {
        public long BookId { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string OriginDate { get; set; }

        public string RelicAbbreviation { get; set; }

        public string SourceAbbreviation { get; set; }

        public IList<string> Notes { get; set; }

        public PageWithContextContract PageResultContext { get; set; }

        public VerseResultContext VerseResultContext { get; set; }

        public BibleVerseResultContext BibleVerseResultContext { get; set; }
    }

    public class PageWithContextContract : PageContract
    {
        public KwicStructure ContextStructure { get; set; }
    }
}