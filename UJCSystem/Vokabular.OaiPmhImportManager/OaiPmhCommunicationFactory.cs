using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vokabular.ProjectImport.Shared.Options;

namespace Vokabular.OaiPmhImportManager
{
    /// <summary>
    /// Factory class for creating OaiPmhCommunicationClients
    /// </summary>
    public class OaiPmhCommunicationFactory
    {
        private readonly IOptions<OaiPmhClientOption> m_oaiPmhClientOption;
        private readonly ILogger<OaiPmhCommunicationClientWrapper> m_logger;

        public OaiPmhCommunicationFactory(IOptions<OaiPmhClientOption> oaiPmhOptions, ILogger<OaiPmhCommunicationClientWrapper> logger)
        {
            m_oaiPmhClientOption = oaiPmhOptions;
            m_logger = logger;
        }
        
        /// <summary>
        /// Creates a disposable client to communicate with the OAI-PMH API of the given resource.
        /// </summary>
        /// <param name="url">URL of resource with OAI-PMH API.</param>
        /// <returns></returns>
        public OaiPmhCommunicationClientWrapper CreateClient(string url)
        {
            return new OaiPmhCommunicationClientWrapper(m_oaiPmhClientOption.Value, url, m_logger);
        }       
    }
}