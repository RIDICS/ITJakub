using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Books;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.SynchronizedReading.ViewModel;

namespace ITJakub.MobileApps.Client.SynchronizedReading.DataService
{
    public class ReaderDataService
    {
        private readonly SynchronizationManager m_synchronizationManager;
        private readonly ReaderTaskManager m_taskManager;
        private readonly BookManager m_bookManager;
        private readonly IErrorService m_errorService;

        public ReaderDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_errorService = applicationCommunication.ErrorService;
            m_bookManager = new BookManager(Book.DataService);
            m_synchronizationManager = new SynchronizationManager(applicationCommunication, m_bookManager);
            m_taskManager = new ReaderTaskManager(applicationCommunication, m_bookManager);
        }

        public IErrorService ErrorService
        {
            get { return m_errorService; }
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

        public void GetPageList(Action<ObservableCollection<PageViewModel>, Exception> callback)
        {
            m_bookManager.GetPageList(callback);
        }

        public void GetPageAsRtf(Action<string, Exception> callback)
        {
            m_bookManager.GetPageAsRtf(callback);
        }

        public void GetPagePhoto(Action<BitmapImage, Exception> callback)
        {
            m_bookManager.GetPagePhoto(callback);
        }

        public void SetTask(string data)
        {
            m_taskManager.SetTask(data);
        }

        public void GetCurrentPage(Action<string> callback)
        {
            callback(m_bookManager.PageId);
        }

        public void UpdateCurrentPage(string pageId, Action<Exception> callback)
        {
            m_synchronizationManager.UpdateCurrentPage(pageId, callback);
        }

        public void SetCurrentBook(string bookGuid, string pageId)
        {
            m_bookManager.BookGuid = bookGuid;
            m_bookManager.PageId = pageId;
        }

        public string GetCurrentBookGuid()
        {
            return m_bookManager.BookGuid;
        }

        public void CreateTask(string taskName, string defaultPageId, Action<Exception> callback)
        {
            m_taskManager.CreateTask(taskName, defaultPageId, callback);
        }
    }
}
