namespace ITJakub.MobileApps.Client.Core.Communication.Client
{
    public class MobileAppsServiceClientManager
    {
        public string CommunicationToken { get { return m_clientMessageInspector.CommunicationToken; } }

        private readonly ClientMessageInspector m_clientMessageInspector = new ClientMessageInspector();

        public void UpdateCommunicationToken(string communicationToken)
        {            
            m_clientMessageInspector.CommunicationToken = communicationToken;
        }

        public void Logout()
        {            
            m_clientMessageInspector.CommunicationToken = string.Empty;
        }

        public MobileAppsServiceClient GetClient()
        {
            var client =  new MobileAppsServiceClient(m_clientMessageInspector);
            return client;
        }
    }
}