using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Books.Service.Client;
using ITJakub.MobileApps.Client.Books.ViewModel;
using ITJakub.MobileApps.MobileContracts;

namespace ITJakub.MobileApps.Client.Books.Manager
{
    public class BookManager
    {
        private readonly IServiceClient m_serviceClient;
        private readonly BookModel m_currentBook;

        public BookManager(IServiceClient serviceClient)
        {
            m_serviceClient = serviceClient;
            m_currentBook = new BookModel();
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
            set
            {
                m_currentBook.Author = value.Author;
                m_currentBook.Guid = value.Guid;
                m_currentBook.Title = value.Title;
                m_currentBook.Year = value.Year;
            }
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

        public async void GetPageList(string bookGuid, Action<ObservableCollection<BookPageViewModel>, Exception> callback)
        {
            try
            {
                var list = await m_serviceClient.GetPageListAsync(bookGuid);
                var viewModels = new ObservableCollection<BookPageViewModel>(list.Select(page => new BookPageViewModel
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
                using (var pageStream = await m_serviceClient.GetPageAsRtfAsync(bookGuid, pageId))
                using (var streamReader = new StreamReader(pageStream))
                {
                    var text = streamReader.ReadToEnd();
                    callback(text, null);
                }
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
                using (var stream = await m_serviceClient.GetPagePhotoAsync(bookGuid, pageId))
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(memoryStream.AsRandomAccessStream());

                    callback(bitmapImage, null);
                }
            }
            catch (MobileCommunicationException exception)
            {
                callback(null, exception);
            }
        }
    }
}
