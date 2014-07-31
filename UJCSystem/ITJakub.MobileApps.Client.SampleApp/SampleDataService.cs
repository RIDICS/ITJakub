using System;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.SampleApp
{
    public class SampleDataService
    {
        private readonly ISynchronizeCommunication m_synchronizeManager;

        public SampleDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_synchronizeManager = applicationCommunication;
        }

        public void GetData(Action<Exception, string> callback)
        {
            callback(null, "Data z DataService");
        }
    }
}
