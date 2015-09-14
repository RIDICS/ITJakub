using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Books.Enum;
using ITJakub.MobileApps.Client.Books.Manager;
using ITJakub.MobileApps.Client.Books.Service.Client;
using ITJakub.MobileApps.Client.Books.ViewModel;
using ITJakub.MobileApps.MobileContracts;

namespace ITJakub.MobileApps.Client.Books.Service
{
    public class DataService : IDataService
    {
        private readonly BookManager m_bookManager;
        private readonly IBookServiceClient m_serviceClient;

        public DataService(BookManager bookManager, IBookServiceClient serviceClient)
        {
            m_bookManager = bookManager;
            m_serviceClient = serviceClient;
        }

        public void GetBookList(BookTypeContract category, Action<ObservableCollection<BookViewModel>, Exception> callback)
        {
            m_bookManager.GetBookList(category, callback);
        }

        public void SearchForBook(BookTypeContract category, SearchDestinationContract searchDestination, string query, Action<ObservableCollection<BookViewModel>, Exception> callback)
        {
            m_bookManager.SearchForBook(category, searchDestination, query, callback);
        }

        public void GetPageList(string bookGuid, Action<ObservableCollection<PageViewModel>, Exception> callback)
        {
            m_bookManager.GetPageList(bookGuid, callback);
        }

        public void GetPageAsRtf(string bookGuid, string pageId, Action<string, Exception> callback)
        {
            m_bookManager.GetPageAsRtf(bookGuid, pageId, callback);
        }

        public void GetPagePhoto(string bookGuid, string pageId, Action<BitmapImage, Exception> callback)
        {
            m_bookManager.GetPagePhoto(bookGuid, pageId, callback);
        }

        public void GetBookInfo(string bookGuid, Action<BookViewModel, Exception> callback)
        {
            m_bookManager.GetBookInfo(bookGuid, callback);
        }

        public void SetCurrentBook(BookViewModel book)
        {
            m_bookManager.CurrentBook = book;
        }

        public void GetCurrentBook(Action<BookViewModel> callback)
        {
            callback(m_bookManager.CurrentBook);
        }

        public void SetMode(ReaderMode readerMode)
        {
            m_bookManager.ReaderMode = readerMode;
        }

        public void GetMode(Action<ReaderMode> callback)
        {
            callback(m_bookManager.ReaderMode);
        }

        public void UpdateEndpointAddress(string address)
        {
            var serviceClient = m_serviceClient as BookServiceClient;
            if (serviceClient != null)
                serviceClient.UpdateEndpointAddress(address);
        }
    }
}