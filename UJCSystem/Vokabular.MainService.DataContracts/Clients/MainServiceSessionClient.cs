using System;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceSessionClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly MainServiceRestClient m_client;

        public MainServiceSessionClient(MainServiceRestClient client)
        {
            m_client = client;
        }

        public void UploadResource(string sessionId, Stream data, string fileName)
        {
            try
            {
                var uriPath = $"ProjectImportSession/{sessionId}/resource";
                m_client.PostStreamAsForm<object>(uriPath, data, fileName);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void ProcessSessionAsImport(string sessionId, NewBookImportContract request)
        {
            try
            {
                var requestTimeout = new TimeSpan(0, 10, 0); // Import is long running operation
                m_client.Post<object>($"ProjectImportSession/{sessionId}", request, requestTimeout);
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
