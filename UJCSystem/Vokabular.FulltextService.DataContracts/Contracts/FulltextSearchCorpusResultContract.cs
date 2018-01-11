using System.Collections.Generic;

namespace Vokabular.FulltextService.DataContracts.Contracts
{
    public class FulltextSearchCorpusResultContract
    {
        public long Count { get; set; }
        public List<string> TextList { get; set; }
    }
}