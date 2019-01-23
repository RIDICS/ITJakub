using System;
using System.Configuration;
using ITJakub.SearchService.DataContracts;
using Vokabular.Authentication.Client;
using Vokabular.FulltextService.DataContracts.Clients;

namespace ITJakub.FileProcessing.Core.Communication
{
    public class FileProcessingCommunicationProvider
    {
        private const string FulltextServiceEndpointName = "FulltextServiceEndpoint";
        private const string AuthenticationEndpointName = "AuthenticationService";
        private const string ApiAccessKeyHash = "ApiAccessKeyHash";
        private const string ApiAccessKeyName = "ApiAccessKeyName";

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

        public AuthenticationClient GetAuthenticationServiceClient()
        {
            var url = ConfigurationManager.AppSettings[AuthenticationEndpointName] ?? throw new ArgumentException("Authentication service endpoint URL not found");
            var uri = new Uri(url);
            var keyHash = ConfigurationManager.AppSettings[ApiAccessKeyHash] ?? throw new ArgumentException("Authentication API Access credentials not found");
            var keyName = ConfigurationManager.AppSettings[ApiAccessKeyName] ?? throw new ArgumentException("Authentication API Access credentials not found");
           
            var client = new AuthenticationClient(uri, keyName, keyHash);
            return client;
        }
    }
}
