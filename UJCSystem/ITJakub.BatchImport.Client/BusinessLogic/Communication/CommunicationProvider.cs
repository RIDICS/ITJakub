using System;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.ITJakubService.DataContracts.Clients;
using Vokabular.MainService.DataContracts.Clients;

namespace ITJakub.BatchImport.Client.BusinessLogic.Communication
{
    public class CommunicationProvider
    {
        private readonly MainServiceRestClient m_mainServiceRestClient;
        private const string EncryptedEndpointName = "ItJakubServiceEncrypted";
        private const string MainServiceEndpointName = "ItJakubService";
        private const string MainServiceEndpointNameAuthenticated = "ItJakubService.Authenticated";
        private const string StreamedServiceEndpointName = "ItJakubServiceStreamed";
        //private const string StreamedServiceEndpointNameAuthenticated = "ItJakubServiceStreamed.Authenticated";
        private const string StreamedServiceEndpointNameAuthenticated = "ItJakubServiceStreamed";

        public CommunicationProvider(MainServiceRestClient mainServiceRestClient)
        {
            m_mainServiceRestClient = mainServiceRestClient;
        }

        public MainServiceRestClient GetMainServiceClient()
        {
            return m_mainServiceRestClient;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="authenticationToken">Password or communication token</param>
        /// <returns></returns>
        public IItJakubService GetAuthenticatedClient(string username, string authenticationToken)
        {
            var client = new ItJakubServiceClient(MainServiceEndpointNameAuthenticated);
            if (client.ClientCredentials == null)
            {
                throw new ArgumentException("Cannot set credentials for client");
            }

            client.ClientCredentials.UserName.UserName = username;
            client.ClientCredentials.UserName.Password = authenticationToken;
            return client;
        }

        public IItJakubService GetUnsecuredClient()
        {
            var client = new ItJakubServiceClient(MainServiceEndpointName);
            return client;
        }

        public ItJakubServiceEncryptedClient GetEncryptedClient()
        {
            var client = new ItJakubServiceEncryptedClient(EncryptedEndpointName);
            return client;
        }

        public ItJakubServiceStreamedClient GetStreamingClient()
        {
            var client = new ItJakubServiceStreamedClient(StreamedServiceEndpointName);
            return client;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="authenticationToken">Password or communication token</param>>
        /// <returns></returns>
        public ItJakubServiceStreamedClient GetStreamingClientAuthenticated(string username, string authenticationToken)
        {
            var client = new ItJakubServiceStreamedClient(StreamedServiceEndpointNameAuthenticated);
            if (client.ClientCredentials == null)
            {
                throw new ArgumentException("Cannot set credentials for client");
            }
            client.ClientCredentials.UserName.UserName = username;
            client.ClientCredentials.UserName.Password = authenticationToken;
            return client;
        }


    }
}
