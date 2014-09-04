using System;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.SampleApp
{
    public class SampleDataService
    {
        private readonly ISynchronizeCommunication m_synchronizeManager;
        private readonly IPollingService m_pollingService;

        public SampleDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_synchronizeManager = applicationCommunication;
            m_pollingService = applicationCommunication.GetPollingService();
        }

        public void GetData(Action<string, Exception> callback)
        {
            callback("Data z DataService", null);
        }

        public void StartObjectPolling(Action<object, Exception> callback)
        {
            m_pollingService.RegisterForSynchronizedObjects(PollingInterval.Medium, ApplicationType.SampleApp, "ObjectTyp",
                (list, exception) =>
                {
                    if (exception != null)
                        return;

                    callback(list, null);
                });
        }
    }
}
