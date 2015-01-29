using System;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.SynchronizedReading.ViewModel;

namespace ITJakub.MobileApps.Client.SynchronizedReading.DataService
{
    public class ReaderDataService
    {
        private readonly SynchronizationManager m_synchronizationManager;

        public ReaderDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_synchronizationManager = new SynchronizationManager(applicationCommunication);
        }

        public void StartPollingUpdates(Action<UpdateViewModel, Exception> callback)
        {
            m_synchronizationManager.StartPollingUpdates(callback);
        }

        public void StopPolling()
        {
            m_synchronizationManager.StopPolling();
        }

        public void SendUpdate(UpdateViewModel update, Action<Exception> callback)
        {
            m_synchronizationManager.SendUpdate(update, callback);
        }
    }
}
