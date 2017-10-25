using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.DataContracts.Contracts;

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

        public ResultContract CreateSnapshotResource(SnapshotResource snapshotResource)
        {
            var client = m_communicationProvider.GetElasticClient();
            var response = client.Index(snapshotResource, idx => idx.Index(Index).Type(Type));
            if (response.Created)
            {
                return new ResultContract { Id = response.Id };
            }
            return new ResultContract { Id = null };
        }
    }
}