using System;
using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts.OaiPmh
{
    public class OaiPmhRepositoryInfoContract
    {
        public string[] AdminMails { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public DateTime EarliestDateTime { get; set; }
        public string Granularity { get; set; }
        public string Name { get; set; }
        public IList<MetadataFormatContract> MetadataFormats { get; set; }
        public IList<SetContract> Sets { get; set; }
    }
}
