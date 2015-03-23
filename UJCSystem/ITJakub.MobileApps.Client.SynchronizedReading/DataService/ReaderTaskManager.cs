using System;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.Client.SynchronizedReading.DataContract;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.SynchronizedReading.DataService
{
    public class ReaderTaskManager
    {
        private readonly ISynchronizeCommunication m_applicationCommunication;
        private readonly BookManager m_bookManager;

        public ReaderTaskManager(ISynchronizeCommunication applicationCommunication, BookManager bookManager)
        {
            m_applicationCommunication = applicationCommunication;
            m_bookManager = bookManager;
        }

        public async void CreateTask(string taskName, string defaultPageId, Action<Exception> callback)
        {
            var taskContract = new SelectedBookTaskContract
            {
                BookGuid = m_bookManager.BookGuid,
                DefaultPageId = defaultPageId
            };
            var serializedTask = JsonConvert.SerializeObject(taskContract);

            try
            {
                await m_applicationCommunication.CreateTaskAsync(ApplicationType.SynchronizedReading, taskName, serializedTask);
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }

        public void SetTask(string data)
        {
            var taskContract = JsonConvert.DeserializeObject<SelectedBookTaskContract>(data);

            m_bookManager.BookGuid = taskContract.BookGuid;
            m_bookManager.PageId = taskContract.DefaultPageId;
        }
    }
}