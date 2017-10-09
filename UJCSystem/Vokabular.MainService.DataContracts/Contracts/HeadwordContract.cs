using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class HeadwordContract
    {
        public long Id { get; set; }
        public long VersionId { get; set; }
        public int VersionNumber { get; set; }

        public string ExternalId { get; set; }
        public string DefaultHeadword { get; set; }
        public string Sorting { get; set; }
        public List<HeadwordItemContract> HeadwordItems { get; set; }
    }

    public class HeadwordItemContract
    {
        public long Id { get; set; }
        public string Headword { get; set; }
        public string HeadwordOriginal { get; set; }
        public long ResourcePageId { get; set; }
    }
}
