using System;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.SampleApp.Service
{
    public class SampleDataService
    {
        private readonly SampleManager m_sampleManager;

        public SampleDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_sampleManager = new SampleManager(applicationCommunication);
        }

        public void GetData(Action<string, Exception> callback)
        {
            callback("Data z DataService", null);
        }

        public void StartObjectPolling(Action<object, Exception> callback)
        {
            m_sampleManager.RegisterForSynchronizedObjects(callback);
            
        }

        public void SaveTask(string data, Action<Exception> callback)
        {
            m_sampleManager.SaveTask(data, callback);
        }
    }
}
