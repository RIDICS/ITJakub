using ITJakub.SearchService.DataContracts;
using Vokabular.FulltextService.DataContracts.Clients;

namespace ITJakub.FileProcessing.Core.Communication
{
    public class FileProcessingCommunicationProvider
    {
        private readonly FulltextServiceClient m_fulltextServiceClient;

        public FileProcessingCommunicationProvider(FulltextServiceClient fulltextServiceClient)
        {
            m_fulltextServiceClient = fulltextServiceClient;
        }


        public SearchServiceClient GetSearchServiceClient()
        {
            return new SearchServiceClient();
        }

        public FulltextServiceClient GetFulltextServiceClient()
        {
            return m_fulltextServiceClient;
        }
    }
}
