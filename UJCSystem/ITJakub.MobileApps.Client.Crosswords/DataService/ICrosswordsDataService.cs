using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Crosswords.DataService
{
    public interface ICrosswordsDataService
    {
    }

    public class CrosswordsDataService : ICrosswordsDataService
    {
        private readonly ISynchronizeCommunication m_applicationCommunication;

        public CrosswordsDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
        }
    }
}