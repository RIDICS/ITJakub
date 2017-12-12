using System;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.DataContracts.Contracts;

namespace Vokabular.FulltextService.Core.Managers
{
    public class SnapshotResourceManager : ElasticsearchManagerBase
    {
        public SnapshotResourceManager(CommunicationProvider communicationProvider) : base(communicationProvider) {}
        

        public ResultContract CreateSnapshotResource(SnapshotResourceContract snapshotResourceContract)
        {
            var client = CommunicationProvider.GetElasticClient();
            
            var response = client.Index(snapshotResourceContract, idx => idx.Index(SnapshotIndex).Type(SnapshotType));

            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }

            return new ResultContract { Id = response.Id };
        }
    }
}