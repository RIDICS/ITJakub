using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vokabular.ProjectImport.Shared.Options;

namespace Vokabular.OaiPmhImportManager
{
    public class OaiPmhCommunicationFactory
    {
        private readonly IOptions<OaiPmhClientOption> m_oaiPmhClientOption;
        private readonly ILogger<OaiPmhCommunicationClientWrapper> m_logger;

        public OaiPmhCommunicationFactory(IOptions<OaiPmhClientOption> oaiPmhOptions, ILogger<OaiPmhCommunicationClientWrapper> logger)
        {
            m_oaiPmhClientOption = oaiPmhOptions;
            m_logger = logger;
        }
        
        public OaiPmhCommunicationClientWrapper CreateClient(string url)
        {
            return new OaiPmhCommunicationClientWrapper(m_oaiPmhClientOption.Value, url, m_logger);
        }       
    }
}