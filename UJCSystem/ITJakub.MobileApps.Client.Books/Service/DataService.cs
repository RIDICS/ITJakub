using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ITJakub.MobileApps.Client.Books.ViewModel;
using ITJakub.MobileApps.MobileContracts;

namespace ITJakub.MobileApps.Client.Books.Service
{
    public class DataService
    {
        private readonly BookManager m_bookManager;

        public DataService(BookManager bookManager)
        {
            m_bookManager = bookManager;
        }

        public void GetBookList(CategoryContract category, Action<ObservableCollection<BookViewModel>, Exception> callback)
        {
            m_bookManager.GetBookList(category, callback);
        }

        public void SearchForBook(CategoryContract category, SearchDestinationContract searchDestination, string query, Action<ObservableCollection<BookViewModel>, Exception> callback)
        {
            m_bookManager.SearchForBook(category, searchDestination, query, callback);
        }

        public void GetPageList(string bookGuid, Action<IList<string>, Exception> callback)
        {
            m_bookManager.GetPageList(bookGuid, callback);
        }

        public void GetPageAsRtf(string bookGuid, string pageId, Action<Stream, Exception> callback)
        {
            m_bookManager.GetPageAsRtf(bookGuid, pageId, callback);
        }

        public void GetPagePhoto(string bookGuid, string pageId, Action<Stream, Exception> callback)
        {
            m_bookManager.GetPagePhoto(bookGuid, pageId, callback);
        }
    }
}