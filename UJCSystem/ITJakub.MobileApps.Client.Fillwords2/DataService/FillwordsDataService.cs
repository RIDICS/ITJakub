using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Fillwords2.DataService
{
    public class FillwordsDataService
    {
        private readonly ISynchronizeCommunication m_applicationCommunication;

        public FillwordsDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
        }

        public IErrorService ErrorService
        {
            get { return m_applicationCommunication.ErrorService; }
        }
    }
}
