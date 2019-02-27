using Microsoft.Extensions.Options;
using Vokabular.CommunicationService.OAIPMH;

namespace Vokabular.CommunicationService
{
    public class CommunicationProvider
    {
        private readonly OaiPmhClientSettings m_oaiPmhClientSettings;

        public CommunicationProvider(IOptions<OaiPmhClientSettings> oaiPmhOptions)
        {
            m_oaiPmhClientSettings = oaiPmhOptions.Value;
        }

        public OaiPmhCommunicationClient GetOaiPmhCommunicationClient(string url)
        {
            return new OaiPmhCommunicationClient(m_oaiPmhClientSettings, url);
        }
    }
}
