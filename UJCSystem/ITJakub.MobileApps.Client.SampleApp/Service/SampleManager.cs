using System;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.SampleApp.Service
{
    public class SampleManager
    {
        private readonly ISynchronizeCommunication m_synchronizeManager;
        private readonly IPollingService m_pollingService;

        public SampleManager(ISynchronizeCommunication applicationCommunication)
        {
            m_synchronizeManager = applicationCommunication;
            m_pollingService = applicationCommunication.PollingService;
        }

        public void RegisterForSynchronizedObjects(Action<object, Exception> callback)
        {
            m_pollingService.RegisterForSynchronizedObjects(PollingInterval.Medium, ApplicationType.SampleApp, "ObjectTyp",
                (list, exception) =>
                {
                    if (exception != null)
                        return;

                    callback(list, null);
                });
        }

        public void SaveTask(string data, Action<Exception> callback)
        {
            try
            {
                // serialize data to string
                // var serializedTask = data;

                // send data to server
                // await m_synchronizeManager.CreateTaskAsync(ApplicationType.SampleApp, "Task name", serializedTask);

                // callback to ViewModel - save successful
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                // callback to ViewModel - communication error
                callback(exception);
            }
        }
    }
}