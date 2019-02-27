using System;

namespace Vokabular.CommunicationService.OAIPMH
{
    public class OaiPmhResourceInfo
    {
        public string[] AdminMails { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public DateTime EarliestDateTime { get; set; }
        public string Granularity { get; set; }
        public string Name { get; set; }
    }
}
