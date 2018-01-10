using System.Collections.Generic;

namespace Vokabular.Shared.DataContracts
{
    public class FulltextSearchCorpusResultContract
    {
        public long Count { get; set; }
        public List<string> TextList { get; set; }
    }
}