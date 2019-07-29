using System;
using System.Collections.Generic;

namespace Vokabular.OaiPmhImportManager.Model
{
    public class OaiPmhRepositoryInfo
    {
        public string[] AdminMails { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public DateTime EarliestDateTime { get; set; }
        public granularityType Granularity { get; set; }
        public string Name { get; set; }
        public IList<metadataFormatType> MetadataFormatTypes { get; set; }
        public IList<setType> SetTypes { get; set; }
    }
}
