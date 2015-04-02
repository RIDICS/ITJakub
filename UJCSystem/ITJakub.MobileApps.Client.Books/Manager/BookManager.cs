using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Books.Manager.Cache;
using ITJakub.MobileApps.Client.Books.Service.Client;
using ITJakub.MobileApps.Client.Books.ViewModel;
using ITJakub.MobileApps.MobileContracts;

namespace ITJakub.MobileApps.Client.Books.Manager
{
    public class BookManager
    {
        private const int PhotoCacheSize = 8;
        private const int TextCacheSize = 64;

        private readonly IServiceClient m_serviceClient;
        private readonly PhotoCache m_photoCache;
        private readonly DocumentCache m_documentCache;
        private BookViewModel m_currentBook;

        public BookManager(IServiceClient serviceClient)
        {
            m_serviceClient = serviceClient;
            m_photoCache = new PhotoCache(serviceClient, PhotoCacheSize);
            m_documentCache = new DocumentCache(serviceClient, TextCacheSize);
            m_currentBook = new BookViewModel();
        }

        public BookViewModel CurrentBook
        {
            get 
            { 
                return new BookViewModel
                {
                    Author = m_currentBook.Author,
                    Guid = m_currentBook.Guid,
                    Title = m_currentBook.Title,
                    Year =  m_currentBook.Year
                };
            }
            set { m_currentBook = value; }
        }

        public async void GetBookList(CategoryContract category, Action<ObservableCollection<BookViewModel>, Exception> callback)
        {
            try
            {
                var list = await m_serviceClient.GetBookListAsync(category);
                var viewModelList = list.Select(contract => new BookViewModel
                {
                    Author = contract.Author,
                    Guid = contract.Guid,
                    Title = contract.Title,
                    Year = contract.Year
                });
                callback(new ObservableCollection<BookViewModel>(viewModelList), null);
            }
            catch (MobileCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void SearchForBook(CategoryContract category, SearchDestinationContract searchDestination, string query, Action<ObservableCollection<BookViewModel>, Exception> callback)
        {
            try
            {
                var list = await m_serviceClient.SearchForBookAsync(category, searchDestination, query);
                var viewModelList = list.Select(contract => new BookViewModel
                {
                    Author = contract.Author,
                    Guid = contract.Guid,
                    Title = contract.Title,
                    Year = contract.Year
                });
                callback(new ObservableCollection<BookViewModel>(viewModelList), null);
            }
            catch (MobileCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void GetPageList(string bookGuid, Action<ObservableCollection<PageViewModel>, Exception> callback)
        {
            try
            {
                var list = await m_serviceClient.GetPageListAsync(bookGuid);
                var viewModels = new ObservableCollection<PageViewModel>(list.Select(page => new PageViewModel
                {
                    PageId = page,
                }));

                callback(viewModels, null);
            }
            catch (MobileCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void GetPageAsRtf(string bookGuid, string pageId, Action<string, Exception> callback)
        {
            try
            {
                var textRtf = await m_documentCache.Get(bookGuid, pageId);
                callback(textRtf, null);
            }
            catch (MobileCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void GetPagePhoto(string bookGuid, string pageId, Action<BitmapImage, Exception> callback)
        {
            try
            {
                var photo = await m_photoCache.Get(bookGuid, pageId);
                callback(photo, null);
            }
            catch (MobileCommunicationException exception)
            {
                callback(null, exception);
            }
        }
    }
}
