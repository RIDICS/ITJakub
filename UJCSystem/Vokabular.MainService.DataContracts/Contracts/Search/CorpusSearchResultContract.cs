using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Search.Corpus;

namespace Vokabular.MainService.DataContracts.Contracts.Search
{
    public class CorpusSearchResultContract
    {
        public long BookId { get; set; }

        public string BookXmlId { get; set; }

        public string VersionXmlId { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string OriginDate { get; set; }

        public string Acronym { get; set; }

        public IList<string> Notes { get; set; }

        public PageResultContext PageResultContext { get; set; }

        public VerseResultContext VerseResultContext { get; set; }

        public BibleVerseResultContext BibleVerseResultContext { get; set; }
    }
}