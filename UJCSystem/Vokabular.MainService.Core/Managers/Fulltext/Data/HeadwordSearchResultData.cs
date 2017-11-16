using System.Collections.Generic;

namespace Vokabular.MainService.Core.Managers.Fulltext.Data
{
    public class HeadwordSearchResultData
    {
        public string Headword { get; set; }

        public List<HeadwordDictionaryEntryData> DictionaryEntries { get; set; }
    }

    public class HeadwordDictionaryEntryData
    {
        public long? HeadwordVersionId { get; set; }

        public string HeadwordExternalId { get; set; }

        public long? ProjectId { get; set; }

        public string ProjectExternalId { get; set; }

        //public string Image { get; set; }

        //public long? PageId { get; set; }
    }
}