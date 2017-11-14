using System;
using System.Text;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Managers
{
    public class SnapshotResourceManager : ElasticsearchManagerBase
    {
        public SnapshotResourceManager(CommunicationProvider communicationProvider) : base(communicationProvider) {}
        

        public ResultContract CreateSnapshotResource(SnapshotResourceContract snapshotResourceContract)
        {
            var client = CommunicationProvider.GetElasticClient();
            
            var response = client.Index(snapshotResourceContract, idx => idx.Index(Index).Type(SnapshotType));

            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }

            if (response.Created)
            {
                return new ResultContract { Id = response.Id };
            }
            return new ResultContract { Id = null };
        }
    }
}