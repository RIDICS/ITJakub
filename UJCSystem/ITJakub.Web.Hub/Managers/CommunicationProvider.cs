using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.ITJakubService.DataContracts.Clients;
using ITJakub.Lemmatization.Shared.Contracts;
using Vokabular.MainService.DataContracts.Clients;

namespace ITJakub.Web.Hub.Managers
{
    public class CommunicationProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;

        private const string NewMainServiceEndpointName = "MainService";
        private const string EncryptedEndpointName = "ItJakubServiceEncrypted";
        private const string MainServiceEndpointName = "ItJakubService";
        private const string MainServiceEndpointNameAuthenticated = "ItJakubService.Authenticated";
        private const string StreamedServiceEndpointName = "ItJakubServiceStreamed";
        private const string StreamedServiceEndpointNameAuthenticated = "ItJakubServiceStreamed.Authenticated";
        
        private const string LemmatizationServiceEndpointName = "LemmatizationService";

        public CommunicationProvider(CommunicationConfigurationProvider communicationConfigurationProvider)
        {
            m_configurationProvider = communicationConfigurationProvider;
        }

        public MainServiceClient GetMainServiceClient()
        {
            var uri = m_configurationProvider.GetEndpointUri(NewMainServiceEndpointName);
            return new MainServiceClient(uri);
        }

        public IItJakubService GetAuthenticatedClient(string username, string commToken)
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(MainServiceEndpointNameAuthenticated);
            var binding = m_configurationProvider.GetBasicHttpsBindingUserNameAuthentication();
            var client = new ItJakubServiceClient(binding, endpoint);
            if (client.ClientCredentials == null)
            {
                throw new ArgumentException("Cannot set credentials for client");
            }

            client.ClientCredentials.UserName.UserName = username;
            client.ClientCredentials.UserName.Password = commToken;
            return client;
        }

        public IItJakubService GetUnsecuredClient()
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(MainServiceEndpointName);
            var binding = m_configurationProvider.GetBasicHttpBinding();
            var client = new ItJakubServiceClient(binding, endpoint);
            return client;
        }

        public ItJakubServiceEncryptedClient GetEncryptedClient()
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(EncryptedEndpointName);
            var binding = m_configurationProvider.GetBasicHttpsBindingCertificateAuthentication();
            var behavior = new ClientCredentials();
            behavior.ClientCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByThumbprint, "C787F20847606DC40E50E015A5D2E3A9E59B704C");

            var client = new ItJakubServiceEncryptedClient(binding, endpoint);
            client.Endpoint.Behaviors.Remove<ClientCredentials>();
            client.Endpoint.Behaviors.Add(behavior);

            return client;
        }

        public ItJakubServiceStreamedClient GetStreamingClient()
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(StreamedServiceEndpointName);
            var binding = m_configurationProvider.GetBasicHttpBindingStreamed();
            var client = new ItJakubServiceStreamedClient(binding, endpoint);
            return client;
        }

        public ItJakubServiceStreamedClient GetStreamingClientAuthenticated(string username, string commToken)
        {
            return GetStreamingClient();
            //var endpoint = m_configurationProvider.GetEndpointAddress(StreamedServiceEndpointNameAuthenticated);
            //var binding = m_configurationProvider.GetBasicHttpsBindingStreamed();
            //var client = new ItJakubServiceStreamedClient(binding, endpoint);
            //if (client.ClientCredentials == null)
            //{
            //    throw new ArgumentException("Cannot set credentials for client");
            //}
            //client.ClientCredentials.UserName.UserName = username;
            //client.ClientCredentials.UserName.Password = commToken;
            //return client;
        }


        public LemmatizationServiceClient GetLemmatizationClient()
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(LemmatizationServiceEndpointName);
            var binding = new BasicHttpBinding();
            return new LemmatizationServiceClient(binding, endpoint);
        }
    }
}