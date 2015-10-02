using System;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.ITJakubService.DataContracts.Clients;

namespace ITJakub.Web.Hub
{
    public class CommunicationProvider
    {
        private const string EncryptedEndpointName = "ItJakubServiceEncrypted";
        private const string MainServiceEndpointName = "ItJakubService";
        private const string MainServiceEndpointNameAuthenticated = "ItJakubService.Authenticated";
        private const string StreamedServiceEndpointName = "ItJakubServiceStreamed";
        private const string StreamedServiceEndpointNameAuthenticated = "ItJakubServiceStreamed.Authenticated";

        public IItJakubService GetAuthenticatedClient(string username, string commToken)
        {
            var client = new ItJakubServiceClient("AuthenticatedEndpoint");            
            if (client.ClientCredentials == null)
            {
                throw new ArgumentException("Cannot set credentials for client");
            }

            client.ClientCredentials.UserName.UserName = username;
            client.ClientCredentials.UserName.Password = commToken;
            return client;
        }

        public ItJakubServiceEncryptedClient GetEncryptedClient()
        {
            var client = new ItJakubServiceEncryptedClient(EncryptedEndpointName);
            return client;
        }

        public ItJakubServiceStreamedClient GetStreamingClient()
        {
            var client = new ItJakubServiceStreamedClient();
            return client;
        }

        public IItJakubService GetUnsecuredClient()
        {
            var client = new ItJakubServiceClient();
            return client;
        }
       
    }
}