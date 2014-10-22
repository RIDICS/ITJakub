using System;
using System.Collections.ObjectModel;
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
    }
}