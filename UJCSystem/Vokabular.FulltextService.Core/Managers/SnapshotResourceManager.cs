using System.Text;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Managers
{
    public class SnapshotResourceManager
    {
        private readonly CommunicationProvider m_communicationProvider;
        
        private const string Index = "module"; //TODO rename index and type
        private const string Type = "snapshot";

        public SnapshotResourceManager(CommunicationProvider communicationProvider)
        {
            m_communicationProvider = communicationProvider;
        }

        public ResultContract CreateSnapshotResource(SnapshotResourceContract snapshotResourceContract)
        {
            var client = m_communicationProvider.GetElasticClient();
            
            var response = client.Index(new SnapshotResourceContract{ Text  = snapshotResourceContract.Text, ProjectId = snapshotResourceContract.ProjectId}, idx => idx.Index(Index).Type(Type));

            if (response.Created)
            {
                return new ResultContract { Id = response.Id };
            }
            if (response.OriginalException != null)
            {
                throw response.OriginalException;
            }
            return new ResultContract { Id = null };
        }
    }
}