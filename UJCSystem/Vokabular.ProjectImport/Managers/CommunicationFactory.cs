using Vokabular.OaiPmhImportManager;

namespace Vokabular.ProjectImport.Managers
{
    public class CommunicationFactory
    {
        private readonly OaiPmhCommunicationFactory m_oaiPmhCommunicationFactory;

        public CommunicationFactory(OaiPmhCommunicationFactory oaiPmhCommunicationFactory)
        {
            m_oaiPmhCommunicationFactory = oaiPmhCommunicationFactory;
        }

        public OaiPmhCommunicationClientWrapper CreateOaiPmhCommunicationClient(string url)
        {
            return m_oaiPmhCommunicationFactory.CreateClient(url);
        }
    }
}
