using System.Collections.Generic;

namespace Vokabular.FulltextService.DataContracts.Contracts
{
    public class SnapshotPageIdsResourceContract
    {
        public long ProjectId { get; set; }
        public List<string> PageIds { get; set; }

    }
}