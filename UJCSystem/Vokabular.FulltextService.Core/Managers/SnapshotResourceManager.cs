using System;
using System.Collections.Generic;
using System.Text;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.Core.Helpers;
using Vokabular.FulltextService.DataContracts.Contracts;

namespace Vokabular.FulltextService.Core.Managers
{
    public class SnapshotResourceManager : ElasticsearchManagerBase
    {
        private readonly SnapshotResourceBuilder m_snapshotResourceBuilder;

        public SnapshotResourceManager(CommunicationProvider communicationProvider, SnapshotResourceBuilder snapshotResourceBuilder) : base(communicationProvider)
        {
            m_snapshotResourceBuilder = snapshotResourceBuilder;
        }
        

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

        public ResultContract CreateSnapshotResource(SnapshotPageIdsResourceContract snapshotPageIdsResourceContract)
        {
            var snapshotContract = m_snapshotResourceBuilder.GetSnapshotResourceFromPageIds(snapshotPageIdsResourceContract.PageIds);
            snapshotContract.SnapshotId = snapshotPageIdsResourceContract.SnapshotId;
            snapshotContract.ProjectId = snapshotPageIdsResourceContract.ProjectId;
            
            var result = CreateSnapshotResource(snapshotContract);
            return result;

        }
    }
}