using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

        private readonly IMobileAppsService m_serviceClient;
        private readonly PhotoCache m_photoCache;
        private readonly DocumentCache m_documentCache;
        private BookViewModel m_currentBook;

        public BookManager(IMobileAppsService serviceClient)
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
                    Authors = m_currentBook.Authors,
                    Guid = m_currentBook.Guid,
                    Title = m_currentBook.Title,
                    PublishDate =  m_currentBook.PublishDate
                };
            }
            set { m_currentBook = value; }
        }

        private string GetAuthorStringFromList(IEnumerable<AuthorContract> authors)
        {
            const string authorDelimiter = ", ";
            var stringBuilder = new StringBuilder();
            foreach (var authorContract in authors)
            {
                stringBuilder.Append(authorContract.Name).Append(authorDelimiter);
            }
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Length -= authorDelimiter.Length;
            }

            return stringBuilder.ToString();
        }

        public async void GetBookList(BookTypeContract category, Action<ObservableCollection<BookViewModel>, Exception> callback)
        {
            try
            {
                var list = await m_serviceClient.GetBookListAsync(category);
                var viewModelList = list.Select(contract => new BookViewModel
                {
                    Authors = GetAuthorStringFromList(contract.Authors),
                    Guid = contract.Guid,
                    Title = contract.Title,
                    PublishDate = contract.PublishDate
                });
                callback(new ObservableCollection<BookViewModel>(viewModelList), null);
            }
            catch (MobileCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void SearchForBook(BookTypeContract category, SearchDestinationContract searchDestination, string query, Action<ObservableCollection<BookViewModel>, Exception> callback)
        {
            try
            {
                var list = await m_serviceClient.SearchForBookAsync(category, searchDestination, query);
                var viewModelList = list.Select(contract => new BookViewModel
                {
                    Authors = GetAuthorStringFromList(contract.Authors),
                    Guid = contract.Guid,
                    Title = contract.Title,
                    PublishDate = contract.PublishDate
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
                    Name = page.Name,
                    Position = page.Position,
                    XmlId = page.XmlId
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
