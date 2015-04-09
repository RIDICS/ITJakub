using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Books.ViewModel;
using ITJakub.MobileApps.MobileContracts;

namespace ITJakub.MobileApps.Client.Books.Service
{
    public interface IBookDataService
    {
        void GetPageList(string bookGuid, Action<ObservableCollection<PageViewModel>, Exception> callback);
        void GetPageAsRtf(string bookGuid, string pageId, Action<string, Exception> callback);
        void GetPagePhoto(string bookGuid, string pageId, Action<BitmapImage, Exception> callback);
    }

    public interface IDataService : IBookDataService
    {
        void GetBookList(BookTypeContract category, Action<ObservableCollection<BookViewModel>, Exception> callback);
        void SearchForBook(BookTypeContract category, SearchDestinationContract searchDestination, string query, Action<ObservableCollection<BookViewModel>, Exception> callback);
        void SetCurrentBook(BookViewModel book);
        void GetCurrentBook(Action<BookViewModel> callback);
    }
}