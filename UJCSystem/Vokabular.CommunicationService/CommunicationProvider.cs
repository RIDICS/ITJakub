using Microsoft.Extensions.Options;
using Vokabular.CommunicationService.OAIPMH;
using Vokabular.Shared.Options;

namespace Vokabular.CommunicationService
{
    public class CommunicationProvider
    {
        private readonly OaiPmhClientOption m_oaiPmhClientOption;

        public CommunicationProvider(IOptions<OaiPmhClientOption> oaiPmhOptions)
        {
            m_oaiPmhClientOption = oaiPmhOptions.Value;
        }

        public OaiPmhCommunicationClient GetOaiPmhCommunicationClient(string url)
        {
            return new OaiPmhCommunicationClient(m_oaiPmhClientOption, url);
        }
    }
}
