using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using GalaSoft.MvvmLight.Messaging;
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
        private Popup m_popup;

        private Book()
        {
            Messenger.Default.Register<SelectedPageMessage>(this, BookSelected);

            m_taskCompletition = new TaskCompletionSource<BookPageViewModel>();
            m_navigationService = Container.Current.Resolve<INavigationService>();
        }

        private async Task<BookPageViewModel> StartSelectingBookAsync()
        {
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

        private void BookSelected(SelectedPageMessage message)
        {
            Messenger.Default.Unregister(this);
            Window.Current.SizeChanged -= UpdatePageSize;

            m_popup.IsOpen = false;
            m_taskCompletition.SetResult(message.BookPage);
        }

        public static async Task<BookPageViewModel> SelectBookAsync()
        {
            var book = new Book();
            return await book.StartSelectingBookAsync();
        }

        public static IPublicDataService DataService
        {
            get { return Container.Current.Resolve<IDataService>(); }
        }
    }
}
