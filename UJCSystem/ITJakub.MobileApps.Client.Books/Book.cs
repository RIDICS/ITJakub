using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Books.Enum;
using ITJakub.MobileApps.Client.Books.Message;
using ITJakub.MobileApps.Client.Books.Service;
using ITJakub.MobileApps.Client.Books.View;
using ITJakub.MobileApps.Client.Books.ViewModel;
using Microsoft.Practices.Unity;

namespace ITJakub.MobileApps.Client.Books
{
    public class Book
    {
        private readonly TaskCompletionSource<BookPageViewModel> m_taskCompletition;
        private readonly INavigationService m_navigationService;
        private readonly IDataService m_dataService;
        private Popup m_popup;
        
        private Book()
        {
            Messenger.Default.Register<CloseBookSelectAppMessage>(this, BookSelected);

            m_taskCompletition = new TaskCompletionSource<BookPageViewModel>();
            m_navigationService = Container.Current.Resolve<INavigationService>();
            m_dataService = Container.Current.Resolve<IDataService>();
        }

        private async Task<BookPageViewModel> StartSelectingBookAsync()
        {
            m_dataService.SetMode(ReaderMode.SelectPage);
            m_navigationService.Navigate<SelectBookView>();
            m_popup = m_navigationService.ParentPopup;
            m_popup.IsOpen = true;

            Window.Current.SizeChanged += UpdatePageSize;

            return await m_taskCompletition.Task;
        }

        private void UpdatePageSize(object sender, WindowSizeChangedEventArgs e)
        {
            var page = m_popup.Child as Page;
            if (page == null)
                return;

            page.Width = Window.Current.Bounds.Width;
            page.Height = Window.Current.Bounds.Height;
        }

        private void BookSelected(CloseBookSelectAppMessage message)
        {
            Messenger.Default.Unregister(this);
            Window.Current.SizeChanged -= UpdatePageSize;

            m_popup.IsOpen = false;
            m_taskCompletition.SetResult(message.SelectedBookPage);
        }

        private async Task StartReader(string bookGuid)
        {
            m_dataService.SetMode(ReaderMode.ReadBook);
            m_dataService.SetCurrentBook(new BookViewModel {Guid = bookGuid});
            m_navigationService.Navigate<SelectPageView>();
            m_popup = m_navigationService.ParentPopup;
            m_popup.IsOpen = true;

            Window.Current.SizeChanged += UpdatePageSize;

            await m_taskCompletition.Task;
        }

        public static async Task<BookPageViewModel> SelectBookAsync()
        {
            var book = new Book();
            return await book.StartSelectingBookAsync();
        }

        public static IBookDataService DataService
        {
            get { return Container.Current.Resolve<IDataService>(); }
        }

        public static async void ShowReader(string bookGuid)
        {
            var book = new Book();
            await book.StartReader(bookGuid);
        }
    }
}
