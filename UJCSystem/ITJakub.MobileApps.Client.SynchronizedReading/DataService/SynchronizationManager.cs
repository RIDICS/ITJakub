using System;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.Client.SynchronizedReading.DataContract;
using ITJakub.MobileApps.Client.SynchronizedReading.ViewModel;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.SynchronizedReading.DataService
{
    public class SynchronizationManager
    {
        private const string UpdateType = "Update";
        private const PollingInterval SynchronizationPollingInterval = PollingInterval.Fast;
        private readonly ISynchronizeCommunication m_applicationCommunication;
        private readonly IPollingService m_pollingService;
        private Action<UpdateViewModel, Exception> m_callback;

        public SynchronizationManager(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
            m_pollingService = applicationCommunication.GetPollingService();
        }

        public void StartPollingUpdates(Action<UpdateViewModel,Exception> callback)
        {
            m_callback = callback;
            m_pollingService.RegisterForLatestSynchronizedObject(SynchronizationPollingInterval, ApplicationType.SynchronizedReading, UpdateType, ProcessNewObject);
        }

        public void StopPolling()
        {
            m_pollingService.UnregisterForLatestSynchronizedObject(SynchronizationPollingInterval, ProcessNewObject);
        }

        private void ProcessNewObject(ObjectDetails objectDetails, Exception exception)
        {
            if (exception != null)
            {
                m_callback(null, exception);
                return;
            }
            
            if (objectDetails == null || objectDetails.Data == null)
            {
                return;
            }

            var updateContract = JsonConvert.DeserializeObject<UpdateContract>(objectDetails.Data);
            var updateViewModel = new UpdateViewModel
            {
                SelectionStart = updateContract.SelectionStart,
                SelectionLength = updateContract.SelectionLength
            };

            m_callback(updateViewModel, null);
        }

        public async void SendUpdate(UpdateViewModel update, Action<Exception> callback)
        {
            try
            {
                var updateContract = new UpdateContract
                {
                    SelectionStart = update.SelectionStart,
                    SelectionLength = update.SelectionLength
                };
                var serializedContract = JsonConvert.SerializeObject(updateContract);

                await
                    m_applicationCommunication.SendObjectAsync(ApplicationType.SynchronizedReading, UpdateType,
                        serializedContract);
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }
    }
}