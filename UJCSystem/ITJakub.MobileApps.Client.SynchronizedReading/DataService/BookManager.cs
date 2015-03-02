using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Books.Service;
using ITJakub.MobileApps.Client.SynchronizedReading.ViewModel;

namespace ITJakub.MobileApps.Client.SynchronizedReading.DataService
{
    public class BookManager
    {
        private readonly IBookDataService m_bookDataService;
        private string m_bookGuid;
        private string m_pageId;

        public BookManager(IBookDataService bookDataService)
        {
            m_bookDataService = bookDataService;
        }

        public string BookGuid
        {
            get { return m_bookGuid; }
            set { m_bookGuid = value; }
        }

        public string PageId
        {
            get { return m_pageId; }
            set { m_pageId = value; }
        }
        
        public void SetTask(string data)
        {
            // TODO
            m_bookGuid = string.Empty;
            m_pageId = "1L";
        }

        public void GetPageList(Action<ObservableCollection<PageViewModel>, Exception> callback)
        {
            m_bookDataService.GetPageList(m_bookGuid, (pages, exception) =>
            {
                if (exception != null)
                {
                    callback(null, exception);
                }
                else
                {
                    callback(new ObservableCollection<PageViewModel>(pages.Select(page => new PageViewModel { PageId = page.PageId })), null);
                }
            });
        }

        public void GetPageAsRtf(Action<string, Exception> callback)
        {
            //m_pageId = pageId;
            m_bookDataService.GetPageAsRtf(m_bookGuid, m_pageId, callback);
        }

        public void GetPagePhoto(Action<BitmapImage, Exception> callback)
        {
            m_bookDataService.GetPagePhoto(m_bookGuid, m_pageId, callback);
        }
    }
}