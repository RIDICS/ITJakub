using System.Text;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Managers
{
    public class SnapshotResourceManager
    {
        private readonly CommunicationProvider m_communicationProvider;
        private readonly TextResourceManager m_textResourceManager;

        private const string Index = "module"; //TODO rename index and type
        private const string Type = "snapshot";

        public SnapshotResourceManager(CommunicationProvider communicationProvider, TextResourceManager textResourceManager)
        {
            m_communicationProvider = communicationProvider;
            m_textResourceManager = textResourceManager;
        }

        public ResultContract CreateSnapshotResource(SnapshotPageIdsResourceContract snapshotPageIdsResourceContract)
        {
            var client = m_communicationProvider.GetElasticClient();

            StringBuilder builder = new StringBuilder();
            foreach (var pageId in snapshotPageIdsResourceContract.PageIds)
            {
                var textResource = m_textResourceManager.GetTextResource(pageId); 
                builder.Append(textResource.Text);
            }
            var response = client.Index(new SnapshotResourceContract{ Text  = builder.ToString(), ProjectId = snapshotPageIdsResourceContract.ProjectId}, idx => idx.Index(Index).Type(Type));

            if (response.Created)
            {
                return new ResultContract { Id = response.Id };
            }
            return new ResultContract { Id = null };
        }
    }
}