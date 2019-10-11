using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceSnapshotClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private const string BasePath = "Snapshot";
        private readonly MainServiceRestClient m_client;

        public MainServiceSnapshotClient(MainServiceRestClient client)
        {
            m_client = client;
        }
        
        public long CreateSnapshot(CreateSnapshotContract createSnapshotContract)
        {
            try
            {
                var snapshotId = m_client.Post<long>($"{BasePath}", createSnapshotContract);
                return snapshotId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public SnapshotDetailContract GetSnapshot(long snapshotId)
        {
            try
            {
                var result = m_client.Get<SnapshotDetailContract>($"{BasePath}/{snapshotId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
    }
}
