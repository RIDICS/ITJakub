using Microsoft.Extensions.Options;
using Vokabular.OaiPmhImportManager;
using Vokabular.Shared.Options;

namespace Vokabular.ProjectImport.Managers
{
    public class CommunicationManager
    {
        private readonly OaiPmhClientOption m_oaiPmhClientOption;

        public CommunicationManager(IOptions<OaiPmhClientOption> oaiPmhOptions)
        {
            m_oaiPmhClientOption = oaiPmhOptions.Value;
        }

        public OaiPmhCommunicationClient GetOaiPmhCommunicationClient(string url)
        {
            return new OaiPmhCommunicationClient(m_oaiPmhClientOption, url);
        }
    }
}
