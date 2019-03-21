using System;

namespace Vokabular.OaiPmhImportManager
{
    public class OaiPmhRepositoryInfo
    {
        public string[] AdminMails { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public DateTime EarliestDateTime { get; set; }
        public string Granularity { get; set; }
        public string Name { get; set; }
    }
}
