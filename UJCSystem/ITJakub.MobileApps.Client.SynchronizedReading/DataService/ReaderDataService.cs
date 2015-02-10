using System;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
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

        public void StartPollingControlUpdates(Action<ControlViewModel, Exception> callback)
        {
            m_synchronizationManager.StartPollingControlUpdates(callback);
        }

        public void StopPollingUpdates()
        {
            m_synchronizationManager.StopPollingUpdates();
        }

        public void StopAllPolling()
        {
            m_synchronizationManager.StopPolling();
        }

        public void SendUpdate(UpdateViewModel update, Action<Exception> callback)
        {
            m_synchronizationManager.SendUpdate(update, callback);
        }

        public void PassControl(UserInfo userInfo, Action<Exception> callback)
        {
            m_synchronizationManager.PassControl(userInfo, callback);
        }

        public void TakeReadControl(Action<Exception> callback)
        {
            m_synchronizationManager.TakeReadControl(callback);
        }
    }
}
