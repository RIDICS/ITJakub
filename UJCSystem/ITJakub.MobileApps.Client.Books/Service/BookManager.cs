using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ITJakub.MobileApps.Client.Books.ViewModel;
using ITJakub.MobileApps.MobileContracts;

namespace ITJakub.MobileApps.Client.Books.Service
{
    public class BookManager
    {
        private readonly MockServiceClient m_serviceClient;

        public BookManager(ServiceClient serviceClient)
        {
            m_serviceClient = (MockServiceClient) serviceClient; // todo cast to MockServiceClient only for testing
        }

        public async void GetBookList(CategoryContract category, Action<ObservableCollection<BookViewModel>, Exception> callback)
        {
            try
            {
                var list = await m_serviceClient.GetBookListAsync(category);
                var viewModelList = list.Select(contract => new BookViewModel
                {
                    Author = contract.Author,
                    Id = contract.Guid,
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
                    Id = contract.Guid,
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

        public async void GetPageList(string bookGuid, Action<IList<string>, Exception> callback)
        {
            try
            {
                var list = await m_serviceClient.GetPageListAsync(bookGuid);
                callback(list, null);
            }
            catch (MobileCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void GetPageAsRtf(string bookGuid, string pageId, Action<Stream, Exception> callback)
        {
            try
            {
                var pageStream = await m_serviceClient.GetPageAsRtfAsync(bookGuid, pageId);
                callback(pageStream, null);
            }
            catch (MobileCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void GetPagePhoto(string bookGuid, string pageId, Action<Stream, Exception> callback)
        {
            try
            {
                var photoStream = await m_serviceClient.GetPagePhotoAsync(bookGuid, pageId);
                callback(photoStream, null);
            }
            catch (MobileCommunicationException exception)
            {
                callback(null, exception);
            }
        }
    }
}
