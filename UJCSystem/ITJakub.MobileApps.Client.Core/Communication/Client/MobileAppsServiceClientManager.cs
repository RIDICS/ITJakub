using System;
using System.ServiceModel;

namespace ITJakub.MobileApps.Client.Core.Communication.Client
{
    public class MobileAppsServiceClientManager
    {
        public string CommunicationToken { get { return m_clientMessageInspector.CommunicationToken; } }

        private readonly ClientMessageInspector m_clientMessageInspector = new ClientMessageInspector();

        private EndpointAddress m_endpointAddress;

        //private const string DefaultEndpointAddress = "http://censeo2.felk.cvut.cz/ITJakub.MobileApps.Service/MobileAppsService.svc";
        private const string DefaultEndpointAddress = "http://localhost/ITJakub.MobileApps.Service/MobileAppsService.svc";
        private const string DefaultEndpointAddress = "http://147.32.81.135/ITJakub.MobileApps.Service/MobileAppsService.svc";
        //private const string DefaultEndpointAddress = "http://itjakubmobileapps.cloudapp.net/MobileAppsService.svc";        

        public MobileAppsServiceClientManager()
        {
            m_endpointAddress = new EndpointAddress(new Uri(DefaultEndpointAddress));
        }


        public void UpdateCommunicationToken(string communicationToken)
        {            
            m_clientMessageInspector.CommunicationToken = communicationToken;
        }

        public void UpdateEndpointAddress(string newEndpointAddress)
        {
            m_endpointAddress = new EndpointAddress(newEndpointAddress ?? DefaultEndpointAddress);
        }

        public void Logout()
        {            
            m_clientMessageInspector.CommunicationToken = string.Empty;
        }

        public MobileAppsServiceClient GetClient()
        {
            var client =  new MobileAppsServiceClient(m_clientMessageInspector, m_endpointAddress);
            return client;
        }
    }
}