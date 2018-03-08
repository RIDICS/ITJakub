using System;
using System.Configuration;
using ITJakub.SearchService.DataContracts;
using Vokabular.FulltextService.DataContracts.Clients;

namespace ITJakub.FileProcessing.Core.Communication
{
    public class FileProcessingCommunicationProvider
    {
        private const string FulltextServiceEndpointName = "FulltextServiceEndpoint";

        public SearchServiceClient GetSearchServiceClient()
        {
            return new SearchServiceClient();
        }

        public FulltextServiceClient GetFulltextServiceClient()
        {
            var url = ConfigurationManager.AppSettings[FulltextServiceEndpointName] ?? throw new ArgumentException("Fulltext service endpoint URL not found");
            var uri = new Uri(url);
            return new FulltextServiceClient(uri);
        }
    }
}
