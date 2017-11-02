using System.Collections.Generic;

namespace Vokabular.FulltextService.DataContracts.Contracts
{
    public class SnapshotResourceContract
    {
        public long ProjectId { get; set; }
        public List<string> PageIds { get; set; }

    }
}